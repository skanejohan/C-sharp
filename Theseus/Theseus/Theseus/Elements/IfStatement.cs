using System.Collections.Generic;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class IfStatement : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public class ConditionalSection
        {
            public Expression Expression { get; }
            public Section Section { get; }
            public ConditionalSection(Expression expression, Section section)
            {
                Expression = expression;
                Section = section;
            }
        }

        public IEnumerable<ConditionalSection> Expressions => expressions;
        public Section DefaultSection { get; set; }

        private List<ConditionalSection> expressions;
        public IfStatement()
        {
            expressions = new List<ConditionalSection>();
        }

        public IfStatement(ConditionalSection _if) : this()
        {
            AddExpression(_if);
        }

        public IfStatement(ConditionalSection _if, IEnumerable<ConditionalSection> elseifs) : this()
        {
            AddExpression(_if);
            foreach (var elseif in elseifs)
            {
                AddExpression(elseif);
            }
        }

        public IfStatement(ConditionalSection _if, Section falseSection) : this()
        {
            AddExpression(_if);
            DefaultSection = falseSection;
        }

        public IfStatement(ConditionalSection _if, IEnumerable<ConditionalSection> elseifs, Section falseSection) : this()
        {
            AddExpression(_if);
            foreach (var elseif in elseifs)
            {
                AddExpression(elseif);
            }
            DefaultSection = falseSection;
        }

        public void AddExpression(ConditionalSection cs)
        {
            expressions.Add(cs);
        }

        public string EmitTheseusCode(int indent = 0)
        {
            var s = $"<<if {expressions[0].Expression.EmitTheseusCode()}>>{expressions[0].Section.EmitTheseusCode()}";
            for (var i = 1; i < expressions.Count; i++)
            {
                s += $"<<else if {expressions[i].Expression.EmitTheseusCode()}>>{expressions[i].Section.EmitTheseusCode()}";
            }
            if (DefaultSection != null)
            {
                s += $"<<else>>{DefaultSection.EmitTheseusCode()}";
            }
            s += "<<end>>";
            return s;
        }

        public string EmitJavaScriptCode(int indent = 0)
        {
            var s = $"if ({expressions[0].Expression.EmitJavaScriptCode()}) {{".Indent(indent).AppendNewLine();
            s += expressions[0].Section.EmitJavaScriptCode(indent + 4);
            s += "}".Indent(indent).AppendNewLine();

            for (var i = 1; i < expressions.Count; i++)
            {
                s += $"else if ({expressions[i].Expression.EmitJavaScriptCode()}) {{".Indent(indent).AppendNewLine();
                s += expressions[i].Section.EmitJavaScriptCode(indent + 4);
                s += "}".Indent(indent).AppendNewLine();
            }

            if (DefaultSection != null)
            {
                s += "else {".Indent(indent).AppendNewLine();
                s += DefaultSection.EmitJavaScriptCode(indent + 4);
                s += "}".Indent(indent).AppendNewLine();
            }
            return s;
        }
    }
}
