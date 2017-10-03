using System;
using System.Collections.Generic;
using System.Linq;
using Theseus.Elements.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Action : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
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

        public string EmitTheseusCode(int indent = 0)
        {
            return "[[" + Effects.EmitSourceCode(",") + "]]";
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            return Effects.EmitJavaScript(indent, Environment.NewLine);
        }

    }
}
