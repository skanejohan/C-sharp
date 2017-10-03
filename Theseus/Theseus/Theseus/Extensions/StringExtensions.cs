using System;
using System.Linq;

namespace Theseus.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string s)
        {
            return s == "" ? s : char.ToLowerInvariant(s[0]) + s.Substring(1);
        }

        public static string Indent(this string s, int n)
        {
            return new string(' ', n) + s;
        }

        public static string PrependIfNotEmpty(this string s, string prepend)
        {
            return s == "" ? s: prepend + s; 
        }

        public static string AppendIfNotEmpty(this string s, string append)
        {
            return s == "" ? s : s + append;
        }

        public static string PrependNewLine(this string s)
        {
            return Environment.NewLine + s;
        }

        public static string PrependNewLineIfNotEmpty(this string s)
        {
            return s.PrependIfNotEmpty(Environment.NewLine);
        }

        public static string AppendNewLine(this string s)
        {
            return s + Environment.NewLine;
        }

        public static string AppendNewLine(this string s, int n)
        {
            return s + string.Concat(Enumerable.Repeat(Environment.NewLine, n));
        }

        public static string AppendNewLineIfNotEmpty(this string s)
        {
            return s.AppendIfNotEmpty(Environment.NewLine);
        }

        public static string PrependAndAppendNewLineIfNotEmpty(this string s)
        {
            return s.PrependIfNotEmpty(Environment.NewLine).AppendIfNotEmpty(Environment.NewLine);
        }
    }
}
