using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HpaUtility.Extensions
{
    public static class IntExtensions
    {
        public static bool? ConvertToNullableBool(this int? input)
        {
            return input.HasValue ? Convert.ToBoolean(input) : (bool?)null;
        }

        public static double? TryParseNullableDouble(this int? doubleValue)
        {
            if (!doubleValue.HasValue)
                return null;

            return Convert.ToDouble(doubleValue.Value);
        }
    }
}
