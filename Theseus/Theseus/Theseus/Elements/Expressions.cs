using Theseus.Interfaces;

namespace Theseus.Elements
{
    public class Expression : IElement, ITheseusCodeEmitter, IJavaScriptCodeEmitter
    {
        public SimpleCondition Condition { get; }

        public Expression(SimpleCondition condition)
        {
            Condition = condition;
        }

        public virtual string EmitTheseusCode(int indent = 0)
        {
            return Condition.EmitTheseusCode(indent);
        }

        public virtual string EmitJavaScriptCode(int indent = 0)
        {
            return Condition.EmitJavaScriptCode(indent);
        }
    }

    public class Or : Expression
    {
        public SimpleCondition Condition2 { get; }

        public Or(SimpleCondition condition1, SimpleCondition condition2) : base(condition1)
        {
            Condition2 = condition2;
        }

        public override string EmitTheseusCode(int indent = 0)
        {
            return Condition.EmitTheseusCode(indent) + " or " + Condition2.EmitTheseusCode(0);
        }

        public override string EmitJavaScriptCode(int indent = 0)
        {
            return Condition.EmitJavaScriptCode(indent) + " || " + Condition2.EmitJavaScriptCode(0);
        }
    }

    public class And : Expression
    {
        public SimpleCondition Condition2 { get; }

        public And(SimpleCondition condition1, SimpleCondition condition2) : base(condition1)
        {
            Condition2 = condition2;
        }

        public override string EmitTheseusCode(int indent = 0)
        {
            return Condition.EmitTheseusCode(indent) + " and " + Condition2.EmitTheseusCode(0);
        }

        public override string EmitJavaScriptCode(int indent = 0)
        {
            return Condition.EmitJavaScriptCode(indent) + " && " + Condition2.EmitJavaScriptCode(0);
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

        public override string EmitJavaScriptCode(int indent = 0)
        {
            return "!" + Condition.EmitJavaScriptCode(indent);
        }
    }
}
