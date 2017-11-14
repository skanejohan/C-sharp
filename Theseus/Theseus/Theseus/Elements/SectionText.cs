using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class SectionText : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public string Txt { get; }

        public SectionText(string txt)
        {
            Txt = txt;
        }

        public string EmitTheseusCode(int indent = 0)
        {
            return Txt;
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            var escapedText = Txt.Replace("\"", "&quot;");
            cb.Add($"_s += \"{escapedText}\";");
        }

        // TODO: Semantics check, we probably want to verify matching opening and closing html tags here.
    }
}
