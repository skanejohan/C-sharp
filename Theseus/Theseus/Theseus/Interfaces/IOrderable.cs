using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theseus.Interfaces
{
    public interface IOrderable
    {
        int Order { get; set; }
        IEnumerable<IOrderable> GetDependencies();
    }
}
