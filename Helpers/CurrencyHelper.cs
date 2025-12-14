namespace LindisBakery.Helpers
{
    public static class CurrencyHelper
    {
        public const string CurrencySymbol = "R";
        public const string CurrencyCode = "ZAR";

        // This one method is enough
        public static string FormatAmount(decimal amount)
        {
            // Formats as R1,234.56 for amounts >= 1000
            // and R123.45 for smaller amounts
            return $"{CurrencySymbol}{amount:#,##0.00}";
        }
    }
}