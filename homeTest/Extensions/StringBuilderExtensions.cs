using System.Text;

namespace HpaUtility.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Append(this StringBuilder sb, string format, params object[] args)
        {
            return sb?.Append(string.Format(format, args));
        }

        public static StringBuilder AppendLine(this StringBuilder sb, string format, params object[] args)
        {
            return sb?.AppendLine(string.Format(format, args));
        }
    }
}
