using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotFrameworkDemo.Extensions
{
    public static class TextExtensions
    {
        public static string RemoveStart(this string text, string search)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int startIndex = text.IndexOf(search, StringComparison.OrdinalIgnoreCase);
            if (startIndex >= 0)
            {
                return text.Substring(startIndex + search.Length).Trim();
            }
            return text;
        }

        public static string RemoveEnd(this string text, string search)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int endIndex = text.IndexOf(search, StringComparison.OrdinalIgnoreCase);
            if (endIndex >= 0)
            {
                return text.Substring(0, endIndex).Trim();
            }
            return text;
        }
    }
}