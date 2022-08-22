using System.Text;

namespace QRFocus.Library
{
    public class QRCodeResult
    {
        #region Properties
        public string Value { get; private set; }
        public byte[] ValueArray { get; private set; }
        public int Version { get; internal set; }
        public int Dimension { get; internal set; }
        public QRConstants.ErrorTolerance Tolerance { get; set; } = QRConstants.ErrorTolerance.H;
        #endregion

        #region Constructors
        public QRCodeResult(byte[] data, Encoding optionalEncoding = null)
        {
            ValueArray = data;
            Value = (optionalEncoding ?? Encoding.UTF8).GetString(data);
        }
        #endregion
    }
}