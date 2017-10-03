namespace Theseus.Interfaces
{
    /// <summary>
    /// This interface represents an object that can emit JavaScript code.
    /// </summary>
    public interface IJavaScriptCodeEmitter
    {
        string EmitJavaScriptCode(int indent = 0);
    }
}
