using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Interfaces;

namespace Theseus.Elements.Extensions
{
    public static class EnumerableElementExtensions
    {
        public static string EmitSourceCode(this IEnumerable<IElement> elems)
        {
            return string.Join("", elems.Select(e => (e as ITheseusCodeEmitter)?.EmitTheseusCode()));
        }

        public static string EmitSourceCode(this IEnumerable<IElement> elems, int indent)
        {
            return string.Join("", elems.Select(e => (e as ITheseusCodeEmitter)?.EmitTheseusCode(indent)));
        }

        public static string EmitSourceCode(this IEnumerable<IElement> elems, string delimiter)
        {
            return string.Join(delimiter, elems.Select(e => (e as ITheseusCodeEmitter)?.EmitTheseusCode()));
        }

        public static string EmitSourceCode(this IEnumerable<IElement> elems, int indent, string delimiter)
        {
            return string.Join(delimiter, elems.Select(e => (e as ITheseusCodeEmitter)?.EmitTheseusCode(indent)));
        }

        public static void EmitJavaScript(this IEnumerable<IElement> elems, ISemantics semantics, ICodeBuilder cb)
        {
            foreach(var elem in elems)
            {
                (elem as IJavaScriptCodeEmitter)?.EmitJavaScriptCode(semantics, cb);
            }
        }

        public static void EmitDelimitedJavaScript(this IEnumerable<IElement> elems, ISemantics semantics, ICodeBuilder cb, string delimiter)
        {
            var cb2 = new CodeBuilder(0, 0);
            elems.EmitJavaScript(semantics, cb2);
            cb.Add(string.Join(delimiter, cb2.Lines()));
        }
    }
}
