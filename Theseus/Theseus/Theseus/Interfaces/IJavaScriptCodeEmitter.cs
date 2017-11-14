namespace Theseus.Interfaces
{
    /// <summary>
    /// This interface represents an object that can emit JavaScript code.
    /// </summary>
    public interface IJavaScriptCodeEmitter
    {
        void EmitJavaScriptCode(ISemantics semantics, ICodeBuilder cb);
    }
}
