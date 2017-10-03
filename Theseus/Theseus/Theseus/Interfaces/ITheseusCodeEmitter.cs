namespace Theseus.Interfaces
{
    /// <summary>
    /// This interface represents an object that can emit Theseus code.
    /// </summary>
    public interface ITheseusCodeEmitter
    {
        string EmitTheseusCode(int indent = 0);
    }
}
