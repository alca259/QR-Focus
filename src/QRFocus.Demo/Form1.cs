using QRFocus.Library;
using QRFocus.Library.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace QRFocus.Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.InitialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Work");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK) return;

            var ext = Path.GetExtension(openFileDialog1.SafeFileName);
            var text = string.Empty;

            if (".pdf".Equals(ext, StringComparison.InvariantCultureIgnoreCase))
            {
                var decoder = new PdfService(cfg =>
                {
                    cfg.SIGNATURE_MAX_DEVIATION = 0.75; //0.35 // Desviación
                    cfg.HOR_VERT_SCAN_MAX_DISTANCE = 2.0; //2.0
                    cfg.MODULE_SIZE_DEVIATION = 0.5; //0.5
                    cfg.CORNER_SIDE_LENGTH_DEV = 0.8; //0.8 // Desviación
                    cfg.CORNER_RIGHT_ANGLE_DEV = 0.5; //0.25 // Desviación
                    cfg.ALIGNMENT_SEARCH_AREA = 0.3; //0.3
                });

                var pages = decoder.PageCount(
                    fileName: openFileDialog1.FileName,
                    ppi: 1.0);

                List<QRCodeResult> decoderResult = new List<QRCodeResult>();

                for (int i = 0; i < pages; i++)
                {
                    var forResult = decoder.Decode(
                        fileName: openFileDialog1.FileName,
                        pageIndex: i,
                        ppi: 5.0);

                    decoderResult.AddRange(forResult);
                }

                text = string.Join(Environment.NewLine, decoderResult.Select(s => s.Value));
            }
            else
            {
                var decoder = new QRDecoder(cfg =>
                {
                    cfg.SIGNATURE_MAX_DEVIATION = 0.75; //0.35 // Desviación
                    cfg.HOR_VERT_SCAN_MAX_DISTANCE = 2.0; //2.0
                    cfg.MODULE_SIZE_DEVIATION = 0.5; //0.5
                    cfg.CORNER_SIDE_LENGTH_DEV = 0.8; //0.8 // Desviación
                    cfg.CORNER_RIGHT_ANGLE_DEV = 0.5; //0.25 // Desviación
                    cfg.ALIGNMENT_SEARCH_AREA = 0.3; //0.3
                });

                var decoderResult = decoder.Decode(
                    fileName: openFileDialog1.FileName);
                text = string.Join(Environment.NewLine, decoderResult.Select(s => s.Value));
            }

            label1.Text = text;
        }
    }
}
