using Docnet.Core.Models;
using Docnet.Core.Readers;
using Docnet.Core;
using QRFocus.Library.Abstractions;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System;

namespace QRFocus.Library.Services
{
    public class PdfService : IPdfService
    {
        #region Constructors
        public PdfService()
        {

        }

        public PdfService(Action<QRFocusSettings> settings)
        {
            settings.Invoke(QRFocusSettings.Instance);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Return number of pages on a PDF file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ppi">Page scaling PPI factor.</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int PageCount(string fileName, double ppi = 1.0, string password = null)
        {
            // test argument
            if (fileName == null) throw new ApplicationException("QRDecoder.PDFDecoder File name is null");

            var docReader = DocLib.Instance.GetDocReader(fileName, password, new PageDimensions(ppi));
            return docReader.GetPageCount();
        }

        /// <summary>
        /// Return number of pages on a PDF file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="viewportWidth">Smaller dimension.</param>
        /// <param name="viewportHeight">Larger dimension.</param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <remarks>
        /// Get page dimension options for this particular document. viewportWidth x viewportHeight represents
        /// a viewport to which the document gets scaled to fit without modifying it's aspect
        /// ratio.
        /// </remarks>
        public int PageCount(string fileName, int viewportWidth = 1080, int viewportHeight = 1920, string password = null)
        {
            // test argument
            if (fileName == null) throw new ApplicationException("QRDecoder.PDFDecoder File name is null");

            var docReader = DocLib.Instance.GetDocReader(fileName, password, new PageDimensions(viewportWidth, viewportHeight));
            return docReader.GetPageCount();
        }

        /// <summary>
        /// QR Code decode pdf file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pageIndex">0-Index</param>
        /// <param name="password"></param>
        /// <param name="ppi">Page scaling PPI factor.</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public IEnumerable<QRCodeResult> Decode(string fileName, int pageIndex, double ppi = 1.0, string password = null)
        {
            // test argument
            if (fileName == null) throw new ApplicationException($"{nameof(PdfService)}.{nameof(Decode)} File name is null");

            var docReader = DocLib.Instance.GetDocReader(fileName, password, new PageDimensions(ppi));

            return InternalDecode(fileName, docReader, pageIndex);
        }

        /// <summary>
        /// QR Code decode pdf file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pageIndex">0-Index</param>
        /// <param name="password"></param>
        /// <param name="viewportWidth">Smaller dimension.</param>
        /// <param name="viewportHeight">Larger dimension.</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        /// <remarks>
        /// Get page dimension options for this particular document. viewportWidth x viewportHeight represents
        /// a viewport to which the document gets scaled to fit without modifying it's aspect
        /// ratio.
        /// </remarks>
        public IEnumerable<QRCodeResult> Decode(string fileName, int pageIndex, string password = null, int viewportWidth = 1080, int viewportHeight = 1920)
        {
            // test argument
            if (fileName == null) throw new ApplicationException($"{nameof(PdfService)}.{nameof(Decode)} File name is null");

            var docReader = DocLib.Instance.GetDocReader(fileName, password, new PageDimensions(viewportWidth, viewportHeight));

            return InternalDecode(fileName, docReader, pageIndex);
        }
        #endregion

        #region Pdf aux methods
        private IEnumerable<QRCodeResult> InternalDecode(string fileName, IDocReader docReader, int pageIndex = 0)
        {
            var pageCount = docReader.GetPageCount();
            pageIndex = Math.Abs(pageIndex);
            if (pageIndex >= pageCount) throw new ApplicationException($"{nameof(PdfService)}.{nameof(Decode)} Page index out of bounds.");

            byte[] rawBytes = null;
            int width = 0;
            int height = 0;

            using (IPageReader pageReader = docReader.GetPageReader(pageIndex))
            {
                rawBytes = pageReader.GetImage();

                width = pageReader.GetPageWidth();
                height = pageReader.GetPageHeight();
            }

            var tempFileName = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            var currFileName = Path.GetFileNameWithoutExtension(fileName);
            var imageFileName = Path.Combine(Path.GetDirectoryName(fileName), $"{currFileName}_{tempFileName}.jpg");

            // load pdf to bitmap
            using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                AddBytes(bmp, rawBytes);

                // convert to white & black
                using (Bitmap whiteBlackBmp = ConvertBitmapToBlackAndWhite(bmp, Color.White))
                {
                    whiteBlackBmp.Save(imageFileName, ImageFormat.Jpeg);
                }
            }

            return new QRDecoder().Decode(imageFileName);
        }

        private static void AddBytes(Bitmap bmp, byte[] rawBytes)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            var pNative = bmpData.Scan0;

            Marshal.Copy(rawBytes, 0, pNative, rawBytes.Length);
            bmp.UnlockBits(bmpData);
        }

        private static Bitmap ConvertBitmapToBlackAndWhite(Bitmap inputImage, Color target)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;

            var cloned = new Bitmap(width, height);
            Rectangle rect = new Rectangle(Point.Empty, inputImage.Size);

            using (Graphics gr = Graphics.FromImage(cloned)) // SourceImage is a Bitmap object
            {
                gr.Clear(target);
                gr.DrawImageUnscaledAndClipped(inputImage, rect);

                var gray_matrix = new float[][]
                {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0,      0,      0,      1, 0 },
                    new float[] { 0,      0,      0,      0, 1 }
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(new ColorMatrix(gray_matrix));
                ia.SetThreshold(QRFocusSettings.Instance.BlackAndWhiteThreshold);
                var rc = new Rectangle(0, 0, width, height);
                gr.DrawImage(cloned, rc, 0, 0, width, height, GraphicsUnit.Pixel, ia);
            }

            return cloned;
        }
        #endregion
    }
}