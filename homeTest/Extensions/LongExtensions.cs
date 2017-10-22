using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HpaUtility.Extensions
{
    public static class LongExtensions
    {
        public static int? TryParseNullableInt(this long? value, int? defaultValue = null)
        {
            if (!value.HasValue)
                return defaultValue;
            return (int) value.Value;
        }
    }
}
