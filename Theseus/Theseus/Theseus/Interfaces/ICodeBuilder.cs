using System.Collections.Generic;

namespace Theseus.Interfaces
{
    public interface ICodeBuilder
    {
        void In();
        void Out();
        ICodeBuilder Add();
        ICodeBuilder Add(string s);
        ICodeBuilder Add(bool pred, string s);
        ICodeBuilder Append(string s);
        string ToString();
        IEnumerable<string> Lines();
    }
}
