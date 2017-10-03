using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Section : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public IEnumerable<IElement> Elements { get; }

        public Section(IElement element)
        {
            Elements = new List<IElement> { element };
        }

        public Section(IEnumerable<IElement> elements)
        {
            Elements = elements.ToList();
        }

        public string EmitTheseusCode(int indent = 0)
        {
            return Elements.EmitSourceCode().Indent(indent);
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            return Elements.EmitJavaScript(indent, Environment.NewLine).AppendNewLineIfNotEmpty();
        }
    }
}
