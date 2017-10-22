using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HpaUtility.Extensions
{
    public static class DoubleExtensions
    {
        public static decimal? TryParseNullableDecimal(this double? doubleValue)
        {
            if (!doubleValue.HasValue)
                return null;

            //TODO: Why < 10000000?
            if (doubleValue.Value < 10000000)
            {
                return (decimal)doubleValue.Value;
            }
            return null;
        }

        public static int? TryParseNullableInt(this double? doubleValue)
        {
            if (!doubleValue.HasValue)
                return null;

            return Convert.ToInt32(doubleValue.Value);            
        }
    }
}
