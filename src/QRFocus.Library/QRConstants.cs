namespace QRFocus.Library
{
    public static class QRConstants
    {
        /// <summary>
        /// 0.25
        /// </summary>
        internal const double SIGNATURE_MAX_DEVIATION = 0.35;
        internal const double HOR_VERT_SCAN_MAX_DISTANCE = 2.0;
        /// <summary>
        /// 0.75;
        /// </summary>
        internal const double MODULE_SIZE_DEVIATION = 0.5;
        internal const double CORNER_SIDE_LENGTH_DEV = 0.8;
        /// <summary>
        /// about Sin(4 deg)
        /// </summary>
        internal const double CORNER_RIGHT_ANGLE_DEV = 0.25;
        internal const double ALIGNMENT_SEARCH_AREA = 0.3;

        /// <summary>
        /// Tolerancia a fallos en la lectura
        /// </summary>
        public enum ErrorTolerance
        {
            /// <summary>
            /// Baja
            /// </summary>
            L = 0,
            /// <summary>
            /// Media
            /// </summary>
            M = 1,
            /// <summary>
            /// Medio-Alta
            /// </summary>
            Q = 2,
            /// <summary>
            /// Alta
            /// </summary>
            H = 3
        }

        /// <summary>
		/// Error correction percent (L, M, Q, H)
		/// </summary>
		internal static int[] ErrorTolerancePercent = new int[] { 7, 15, 25, 30 };

        /// <summary>
        /// Codificación de los distintos QRs
        /// </summary>
        public enum Encoding
        {
            Terminator = 0,
            Numeric = 1,
            AlphaNumeric = 2,
            Append = 3,
            Byte = 4,
            FNC1First = 5,
            ECI = 7,
            Kanji = 8,
            FNC1Second = 9
        }
    }
}