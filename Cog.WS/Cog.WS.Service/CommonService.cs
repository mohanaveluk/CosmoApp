using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Cog.WS.Service
{
    public static class CommonService
    {
        public static bool IsNumeric(this string s)
        {
            foreach (var c in s)
            {
                if (!char.IsNumber(c)) return false;
            }
            return true;
        }

        public static bool IsNullOrBlank(this string s)
        {
            return s == null || s.Trim().Length == 0;
        }

        public static string TrimEndAndConvertBlankToNull(this string s)
        {
            if (s == null) return null;
            s = s.TrimEnd();
            return s == string.Empty ? null : s;
        }

        public static string TrimIgnoringNullOrBlank(this string s)
        {
            return String.IsNullOrEmpty(s) ? string.Empty : s.Trim();
        }

        public static string AsJavaScriptStringConstant(this string s)
        {
            return string.Concat('\'', s.Replace("'", @"\'"), '\'');
        }

        public static string AsSqlStringConstant(this string s)
        {
            if (s == null) return "null";
            return string.Concat("'", s.EscapedForSql(), "'");
        }

        public static string EscapedForSql(this string value)
        {
            return value == null ? "" : value.Replace("'", "''");
        }

        public static bool StartsWithIgnoringCase(this string s, string prefix)
        {
            return s.ToUpperInvariant().StartsWith(prefix.ToUpperInvariant());
        }

        public static bool ContainsIgnoringCase(this string s, string infix)
        {
            return s.ToUpperInvariant().Contains(infix.ToUpperInvariant());
        }

        public static bool EqualsIgnoringCase(this string s, string value)
        {
            return s.ToUpperInvariant().Equals(value.ToUpperInvariant());
        }

        public static string TakeUpTo(this string s, int n)
        {
            return n < s.Length ? s.Substring(0, n) : s;
        }

        public static string TakeTrimmedUpTo(this string s, int n)
        {
            return s.TrimStart().TakeUpTo(n).TrimEnd();
        }

        public static string Join(this IEnumerable<string> ss, string separator)
        {
            return string.Join(separator, ss.ToArray());
        }

        public static string ToTitleCase(this string columnName)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(columnName).Replace(" ", "");
        }

        public static string Truncate(this string value)
        {
            if (value == null) return null;
            const int maxLength = 100;
            const string continuation = "...";
            if (value.Length <= maxLength) return value;
            return string.Concat(value.Substring(0, maxLength - continuation.Length), continuation);
        }
    }
}
