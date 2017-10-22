using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HpaUtility.Extensions
{
    public static class DecimalExtensions
    {
        public static double? ParseDouble(this decimal? decimalValue)
        {
            if (decimalValue.HasValue)
                return (double)decimalValue.Value;
            return null;
        }

        public static double? TryParseNullableDouble(this decimal? decimalValue)
        {
            if (!decimalValue.HasValue)
                return null;

            return Convert.ToDouble(decimalValue.Value);
        }

        public static string ToCurrency(this decimal? decimalValue)
        {
            return !decimalValue.HasValue ? string.Empty : $"{decimalValue.Value:C}";
        }

        public static string ToAmount(this decimal? decimalValue)
        {
            return !decimalValue.HasValue ? string.Empty : string.Format("{0:N}", Convert.ToDecimal(decimalValue));
        }

        public static string ToAmount(this decimal? decimalValue, string prefix)
        {
            return !decimalValue.HasValue ? string.Empty : prefix + string.Format("{0:N}", Convert.ToDecimal(decimalValue));
        }

        public static string ToAmount(this decimal decimalValue)
        {
            return string.Format("{0:N}", (decimalValue));
        }
    }
}
