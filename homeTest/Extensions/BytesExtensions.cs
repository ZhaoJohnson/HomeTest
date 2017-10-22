using System;

namespace HpaUtility.Extensions
{
    public static class BytesExtensions
    {

        public static string ToString(this byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
