namespace RainInAustraliaLib.Extensions
{
    public static class StringExtensions
    {
        public static bool ToBool(this string str) =>
            str == "true";
    }
}
