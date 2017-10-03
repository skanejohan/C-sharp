using Theseus.Extensions;

namespace Theseus.Elements.Extensions
{
    public static class StringExtensions
    {
        public static string CreateHeader(this string s, int indent = 0)
        {
            var dashes = new string('-', s.Length).Indent(indent);
            return s.Indent(indent) + dashes.PrependNewLine().AppendNewLine();
        }
    }
}
