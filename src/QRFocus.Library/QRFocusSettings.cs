using System.Collections.Generic;
using static QRFocus.Library.QRConstants;

namespace QRFocus.Library
{
    public class QRFocusSettings
    {
        private static QRFocusSettings _instance;
        public static QRFocusSettings Instance => _instance ?? (_instance = new QRFocusSettings());

        public double SIGNATURE_MAX_DEVIATION { get; set; } = QRConstants.SIGNATURE_MAX_DEVIATION;
        public double HOR_VERT_SCAN_MAX_DISTANCE { get; set; } = QRConstants.HOR_VERT_SCAN_MAX_DISTANCE;
        public double MODULE_SIZE_DEVIATION { get; set; } = QRConstants.MODULE_SIZE_DEVIATION;
        public double CORNER_SIDE_LENGTH_DEV { get; set; } = QRConstants.CORNER_SIDE_LENGTH_DEV;
        public double CORNER_RIGHT_ANGLE_DEV { get; set; } = QRConstants.CORNER_RIGHT_ANGLE_DEV;
        public double ALIGNMENT_SEARCH_AREA { get; set; } = QRConstants.ALIGNMENT_SEARCH_AREA;

        public Dictionary<ErrorTolerance, int> ErrorTolerancePercent { get; set; } = QRConstants.ErrorTolerancePercent;
    }
}