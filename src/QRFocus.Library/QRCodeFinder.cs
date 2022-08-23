using System;

namespace QRFocus.Library
{
    public class QRCodeFinder
    {
        #region Properties
        // horizontal scan
        public int HRow { get; private set; }
        public int HCol1 { get; private set; }
        public int HCol2 { get; private set; }
        public double HModule { get; private set; }

        // vertical scan
        public int VCol { get; private set; }
        public int VRow1 { get; private set; }
        public int VRow2 { get; private set; }
        public double VModule { get; private set; }

        public double Distance { get; private set; }
        public double ModuleSize { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor during horizontal scan
        /// </summary>
        public QRCodeFinder(
            int row,
            int col1,
            int col2,
            double hModule)
        {
            HRow = row;
            HCol1 = col1;
            HCol2 = col2;
            HModule = hModule;
            Distance = double.MaxValue;
            return;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Match during vertical scan
        /// </summary>
        public void Match(
            int col,
            int row1,
            int row2,
            double vModule)
        {
            // test if horizontal and vertical are not related
            if (col < HCol1 || col >= HCol2 || HRow < row1 || HRow >= row2) return;

            // Module sizes must be about the same
            if (Math.Min(HModule, vModule) < Math.Max(HModule, vModule) * QRFocusSettings.Instance.MODULE_SIZE_DEVIATION) return;

            // calculate distance
            double DeltaX = col - 0.5 * (HCol1 + HCol2);
            double DeltaY = HRow - 0.5 * (row1 + row2);
            double Delta = Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);

            // distance between two points must be less than 2 pixels
            if (Delta > QRFocusSettings.Instance.HOR_VERT_SCAN_MAX_DISTANCE) return;

            // new result is better than last result
            if (Delta < Distance)
            {
                VCol = col;
                VRow1 = row1;
                VRow2 = row2;
                VModule = vModule;
                ModuleSize = 0.5 * (HModule + vModule);
                Distance = Delta;
            }
            return;
        }

        /// <summary>
        /// Horizontal and vertical scans overlap
        /// </summary>
        /// <param name="other">The other match</param>
        public bool Overlap(QRCodeFinder other)
        {
            return other.HCol1 < HCol2 && other.HCol2 >= HCol1 && other.VRow1 < VRow2 && other.VRow2 >= VRow1;
        }
        #endregion

        #region Override
        /// <summary>
        /// Finder to string
        /// </summary>
        public override string ToString()
        {
            return Distance == double.MaxValue
                ? $"Finder: Row: {HRow}, Col1: {HCol1}, Col2: {HCol2}, HModule: {HModule:0.00}"
                : $"Finder: Row: {HRow}, Col: {VCol}, Module: {ModuleSize:0.00}, Distance: {Distance:0.00}";
        }
        #endregion
    }
}