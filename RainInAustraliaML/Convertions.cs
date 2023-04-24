namespace RainInAustraliaML
{
    internal static class Convertions
    {
        /// <summary>
        /// Get sine for cyclical direction data.
        /// </summary>
        /// <param name="degrees">Direction in degrees.</param>
        /// <returns>Value between -1 and 1.</returns>
        public static float WindDirectionSin(float degrees) =>
            (float)Math.Sin(2 * Math.PI * degrees / 360);

        /// <summary>
        /// Get cosine for cyclical direction data.
        /// </summary>
        /// <param name="degrees">Direction in degrees.</param>
        /// <returns>Value between -1 and 1.</returns>
        public static float WindDirectionCos(float degrees) =>
            (float)Math.Cos(2 * Math.PI * degrees / 360);

        /// <summary>
        /// Get sine for cyclical day of year data.
        /// </summary>
        /// <param name="dayOfYear">Day number (0 to 365).</param>
        /// <returns>Value between -1 and 1.</returns>
        public static float DayOfYearSin(float dayOfYear) => 
            (float)Math.Sin(2 * Math.PI * dayOfYear / 365);

        /// <summary>
        /// Get cosine for cyclical day of year data.
        /// </summary>
        /// <param name="dayOfYear">Day number (0 to 365).</param>
        /// <returns>Value between -1 and 1.</returns>
        public static float DayOfYearCos(float dayOfYear) =>
            (float)Math.Cos(2 * Math.PI * dayOfYear / 365);
    }
}
