using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Action : IElement, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public IEnumerable<Effect> Effects { get; }

        public Action(Effect effect)
        {
            Effects = new List<Effect> { effect };
        }

        public Action(IEnumerable<Effect> effects)
        {
            Effects = effects.ToList();
        }

        public virtual void BuildSemantics(ISemantics semantics)
        {
        }

        public virtual void CheckSemantics(ISemantics semantics)
        {
            foreach (var e in Effects)
            {
                e.CheckSemantics(semantics);
            }
        }

        public string EmitTheseusCode(int indent = 0)
        {
            return "[[" + Effects.EmitSourceCode(",") + "]]";
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            Effects.EmitJavaScript(semantics, cb);
        }

    }
}
