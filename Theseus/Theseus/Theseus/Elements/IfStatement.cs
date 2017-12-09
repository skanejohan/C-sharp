using System.Collections.Generic;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class IfStatement : IElement, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
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

        public virtual void BuildSemantics(ISemantics semantics)
        {
        }

        public virtual void CheckSemantics(ISemantics semantics)
        {
            foreach (var cs in Expressions)
            {
                cs.Expression.CheckSemantics(semantics);
                cs.Section.CheckSemantics(semantics);
            }
            DefaultSection?.CheckSemantics(semantics);
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

        public void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            cb.Add("if (");
            expressions[0].Expression.EmitJavaScriptCode(semantics, cb);
            cb.Append(") {").In();
            expressions[0].Section.EmitJavaScriptCode(semantics, cb);
            cb.Out();
            cb.Add("}");

            for (var i = 1; i < expressions.Count; i++)
            {
                cb.Add("else if (");
                expressions[i].Expression.EmitJavaScriptCode(semantics, cb);
                cb.Append(") {").In();
                expressions[i].Section.EmitJavaScriptCode(semantics, cb);
                cb.Out();
                cb.Add("}");
            }

            if (DefaultSection != null)
            {
                cb.Add("else {").In();
                DefaultSection.EmitJavaScriptCode(semantics, cb);
                cb.Out();
                cb.Add("}");
            }
        }
    }
}
