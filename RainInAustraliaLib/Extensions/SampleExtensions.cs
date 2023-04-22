using System.Globalization;

namespace RainInAustraliaLib.Extensions
{
    public static class SampleExtensions
    {
        #region DTO to Model
        /// <summary>
        /// Convert <see cref="bool"/> to bi-polar single precision value.
        /// </summary>
        /// <param name="boolean">Input.</param>
        /// <returns>1 if true; -1 if false.</returns>
        public static float ToSampleFloat(this bool boolean) =>
            boolean ? 1.0f : -1.0f;

        /// <summary>
        /// Convert <see cref="DateOnly"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="date">Input.</param>
        /// <returns>Correctly formatted date string.</returns>
        public static string ToSampleString(this DateOnly date) =>
            date.ToString("yyyy-MM-dd");
        #endregion

        #region CSV to DTO
        /// <summary>
        /// Convert a <see cref="string"/> represinting a positve or negative value to <see cref="bool"/>.
        /// </summary>
        /// <param name="str">Input.</param>
        /// <returns>True if positive; otherwise false.</returns>
        public static bool ToSampleBool(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;

            if (double.TryParse(str, CultureInfo.InvariantCulture, out double result))
            {
                if (result > 0) return true;
            }

            return false;
        }
        #endregion
    }
}
