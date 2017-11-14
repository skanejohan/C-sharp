namespace Theseus.Interfaces
{
    public interface ISemanticsValidator
    {
        void BuildSemantics(ISemantics semantics);
        void CheckSemantics(ISemantics semantics);
    }
}
