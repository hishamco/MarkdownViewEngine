using System;

namespace MarkdownViewEngine.Extensions
{
    public static class StringExtensions
    {
        public static string ToProper(this string value) =>
            String.Concat(Char.ToUpper(value[0]), value.Substring(1));
    }
}
