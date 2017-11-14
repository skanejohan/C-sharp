using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Section : IElement, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
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

        public virtual void BuildSemantics(ISemantics semantics)
        {
        }

        public virtual void CheckSemantics(ISemantics semantics)
        {
            foreach (var e in Elements)
            {
                (e as ISemanticsValidator)?.CheckSemantics(semantics);
            }
        }

        public string EmitTheseusCode(int indent = 0)
        {
            return Elements.EmitSourceCode().Indent(indent);
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            Elements.EmitJavaScript(semantics, cb);
        }
    }
}
