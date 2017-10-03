using System.Collections.Generic;
using System.Linq;
using Theseus.Interfaces;

namespace Theseus.Elements.Extensions
{
    public static class EnumerableElementExtensions
    {
        public static string EmitSourceCode(this IEnumerable<IElement> elems)
        {
            return string.Join("", elems.Select(e => e.EmitTheseusCode()));
        }

        public static string EmitSourceCode(this IEnumerable<IElement> elems, int indent)
        {
            return string.Join("", elems.Select(e => e.EmitTheseusCode(indent)));
        }

        public static string EmitSourceCode(this IEnumerable<IElement> elems, string delimiter)
        {
            return string.Join(delimiter, elems.Select(e => e.EmitTheseusCode()));
        }

        public static string EmitSourceCode(this IEnumerable<IElement> elems, int indent, string delimiter)
        {
            return string.Join(delimiter, elems.Select(e => e.EmitTheseusCode(indent)));
        }

        public static string EmitJavaScript(this IEnumerable<IElement> elems, int indent, string delimiter)
        {
            return string.Join(delimiter, elems.Select(e => e.EmitJavaScriptCode(indent)));
        }

        public static string EmitJavaScript(this IEnumerable<IElement> elems, string delimiter)
        {
            return string.Join(delimiter, elems.Select(e => e.EmitJavaScriptCode()));
        }
    }
}
