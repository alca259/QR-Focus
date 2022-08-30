using System.Collections.Generic;

namespace QRFocus.Library.Abstractions
{
    public interface IPdfService
    {
        IEnumerable<QRCodeResult> Decode(string fileName, int pageIndex, double ppi = 1.0, string password = null);
        IEnumerable<QRCodeResult> Decode(string fileName, int pageIndex, string password = null, int viewportWidth = 1080, int viewportHeight = 1920);
    }
}
