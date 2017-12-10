using System.Collections.Generic;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class MoodSentences : IElement, IJavaScriptCodeEmitter
    {
        public string Name { get; }
        public List<string> Sentences { get; }

        public MoodSentences(string name, IEnumerable<string> sentences)
        {
            Name = name;
            Sentences = new List<string>(sentences);
        }

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            var gName = $"THESEUS.{GameUtils.GameName.ToUpper()}";
            cb.Add($"{gName}.{Name} = [").In();
            foreach (var s in Sentences)
            {
                cb.Add("\"" + s.Replace("\"", "\\\"") + "\",");
            }
            cb.Out();
            cb.Add("];");
        }
    }
}