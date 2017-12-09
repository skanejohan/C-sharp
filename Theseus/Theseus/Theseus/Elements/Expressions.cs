using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Expression : IElement, ISemanticsValidator, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public SimpleCondition Condition { get; }

        public Expression(SimpleCondition condition)
        {
            Condition = condition;
        }

        public virtual void BuildSemantics(ISemantics semantics)
        {
        }

        public virtual void CheckSemantics(ISemantics semantics)
        {
            Condition.CheckSemantics(semantics);
        }

        public virtual string EmitTheseusCode(int indent = 0)
        {
            return Condition.EmitTheseusCode(indent);
        }

        public virtual void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            Condition.EmitJavaScriptCode(semantics, cb);
        }
    }

    public class Or : Expression
    {
        public SimpleCondition Condition2 { get; }

        public Or(SimpleCondition condition1, SimpleCondition condition2) : base(condition1)
        {
            Condition2 = condition2;
        }

        public override void CheckSemantics(ISemantics semantics)
        {
            base.CheckSemantics(semantics);
            Condition2.CheckSemantics(semantics);
        }

        public override string EmitTheseusCode(int indent = 0)
        {
            return Condition.EmitTheseusCode(indent) + " or " + Condition2.EmitTheseusCode(0);
        }

        public override void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            Condition.EmitJavaScriptCode(semantics, cb);
            cb.Append(" || ");
            Condition2.EmitJavaScriptCode(semantics, cb);
        }
    }

    public class And : Expression
    {
        public SimpleCondition Condition2 { get; }

        public And(SimpleCondition condition1, SimpleCondition condition2) : base(condition1)
        {
            Condition2 = condition2;
        }

        public override void CheckSemantics(ISemantics semantics)
        {
            base.CheckSemantics(semantics);
            Condition2.CheckSemantics(semantics);
        }

        public override string EmitTheseusCode(int indent = 0)
        {
            return Condition.EmitTheseusCode(indent) + " and " + Condition2.EmitTheseusCode(0);
        }

        public override void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            Condition.EmitJavaScriptCode(semantics, cb);
            cb.Append(" && ");
            Condition2.EmitJavaScriptCode(semantics, cb);
        }
    }

    public class Not : Expression
    {
        public Not(SimpleCondition condition) : base(condition)
        {
        }

        public override string EmitTheseusCode(int indent = 0)
        {
            return "not " + Condition.EmitTheseusCode(0);
        }

        public override void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb)
        {
            cb.Append("!");
            Condition.EmitJavaScriptCode(semantics, cb);
        }
    }
}
