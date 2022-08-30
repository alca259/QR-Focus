using System;

namespace QRFocus.Library
{
    public class QRCodeCorner
    {
        #region Properties
        public QRCodeFinder TopLeftFinder { get; private set; }
        public QRCodeFinder TopRightFinder { get; private set; }
        public QRCodeFinder BottomLeftFinder { get; private set; }
        public double TopLineDeltaX { get; private set; }
        public double TopLineDeltaY { get; private set; }
        public double TopLineLength { get; private set; }
        public double LeftLineDeltaX { get; private set; }
        public double LeftLineDeltaY { get; private set; }
        public double LeftLineLength { get; private set; }
        #endregion

        #region Constructor
        public QRCodeCorner(
            QRCodeFinder topLeftFinder,
            QRCodeFinder topRightFinder,
            QRCodeFinder bottomLeftFinder)
        {
            // save three finders
            TopLeftFinder = topLeftFinder;
            TopRightFinder = topRightFinder;
            BottomLeftFinder = bottomLeftFinder;

            // top line slope
            TopLineDeltaX = topRightFinder.VCol - topLeftFinder.VCol;
            TopLineDeltaY = topRightFinder.HRow - topLeftFinder.HRow;

            // top line length
            TopLineLength = Math.Sqrt(TopLineDeltaX * TopLineDeltaX + TopLineDeltaY * TopLineDeltaY);

            // left line slope
            LeftLineDeltaX = bottomLeftFinder.VCol - topLeftFinder.VCol;
            LeftLineDeltaY = bottomLeftFinder.HRow - topLeftFinder.HRow;

            // left line length
            LeftLineLength = Math.Sqrt(LeftLineDeltaX * LeftLineDeltaX + LeftLineDeltaY * LeftLineDeltaY);
            return;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Test QR corner for validity
        /// </summary>
        /// <param name="topLeftFinder"></param>
        /// <param name="topRightFinder"></param>
        /// <param name="bottomLeftFinder"></param>
        /// <returns></returns>
        public static QRCodeCorner CreateCorner(
            QRCodeFinder topLeftFinder,
            QRCodeFinder topRightFinder,
            QRCodeFinder bottomLeftFinder)
        {
            // try all three possible permutation of three finders
            for (int ix = 0; ix < 3; ix++)
            {
                // TestCorner runs three times to test all posibilities
                // rotate top left, top right and bottom left
                if (ix != 0)
                {
                    QRCodeFinder temp = topLeftFinder;
                    topLeftFinder = topRightFinder;
                    topRightFinder = bottomLeftFinder;
                    bottomLeftFinder = temp;
                }

                // top line slope
                double topLineDeltaX = topRightFinder.VCol - topLeftFinder.VCol;
                double topLineDeltaY = topRightFinder.HRow - topLeftFinder.HRow;

                // left line slope
                double leftLineDeltaX = bottomLeftFinder.VCol - topLeftFinder.VCol;
                double leftLineDeltaY = bottomLeftFinder.HRow - topLeftFinder.HRow;

                // top line length
                double topLineLength = Math.Sqrt(topLineDeltaX * topLineDeltaX + topLineDeltaY * topLineDeltaY);

                // left line length
                double leftLineLength = Math.Sqrt(leftLineDeltaX * leftLineDeltaX + leftLineDeltaY * leftLineDeltaY);

                // the short side must be at least 80% of the long side
                if (Math.Min(topLineLength, leftLineLength) < QRFocusSettings.Instance.CORNER_SIDE_LENGTH_DEV * Math.Max(topLineLength, leftLineLength))
                    continue;

                // top line vector
                double topLineSin = topLineDeltaY / topLineLength;
                double topLineCos = topLineDeltaX / topLineLength;

                // rotate lines such that top line is parallel to x axis
                // left line after rotation
                double newLeftX = topLineCos * leftLineDeltaX + topLineSin * leftLineDeltaY;
                double newLeftY = -topLineSin * leftLineDeltaX + topLineCos * leftLineDeltaY;

                // new left line X should be zero (or between +/- 4 deg)
                if (Math.Abs(newLeftX / leftLineLength) > QRFocusSettings.Instance.CORNER_RIGHT_ANGLE_DEV)
                    continue;

                // swap top line with left line
                if (newLeftY < 0)
                {
                    // swap top left with bottom right
                    QRCodeFinder tempFinder = topRightFinder;
                    topRightFinder = bottomLeftFinder;
                    bottomLeftFinder = tempFinder;
                }

                return new QRCodeCorner(topLeftFinder, topRightFinder, bottomLeftFinder);
            }
            return null;
        }

        /// <summary>
        /// Test QR corner for validity
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public int InitialVersionNumber()
        {
            // version number based on top line
            double topModules = 7;

            // top line is mostly horizontal
            if (Math.Abs(TopLineDeltaX) >= Math.Abs(TopLineDeltaY))
            {
                topModules += TopLineLength * TopLineLength /
                    (Math.Abs(TopLineDeltaX) * 0.5 * (TopLeftFinder.HModule + TopRightFinder.HModule));
            }

            // top line is mostly vertical
            else
            {
                topModules += TopLineLength * TopLineLength /
                    (Math.Abs(TopLineDeltaY) * 0.5 * (TopLeftFinder.VModule + TopRightFinder.VModule));
            }

            // version number based on left line
            double leftModules = 7;

            // Left line is mostly vertical
            if (Math.Abs(LeftLineDeltaY) >= Math.Abs(LeftLineDeltaX))
            {
                leftModules += LeftLineLength * LeftLineLength /
                    (Math.Abs(LeftLineDeltaY) * 0.5 * (TopLeftFinder.VModule + BottomLeftFinder.VModule));
            }

            // left line is mostly horizontal
            else
            {
                leftModules += LeftLineLength * LeftLineLength /
                    (Math.Abs(LeftLineDeltaX) * 0.5 * (TopLeftFinder.HModule + BottomLeftFinder.HModule));
            }

            // version (there is rounding in the calculation)
            int version = ((int)Math.Round(0.5 * (topModules + leftModules)) - 15) / 4;

            // not a valid corner
            if (version < 1 || version > 40)
                //throw new ApplicationException("Corner is not valid (version number must be 1 to 40)");
                return -1;

            // exit with version number
            return version;
        }
        #endregion
    }
}