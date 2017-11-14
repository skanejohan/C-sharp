using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Theseus.Interfaces;

namespace Theseus.Semantics
{
    public class Orderer
    {
        public Orderer()
        {
            orderables = new List<IOrderable>();
        }

        public void AddOrderable(IOrderable orderable)
        {
            orderables.Add(orderable);
        }

        public void Order()
        {
            orderables.ForEach(o => o.Order = 0);
            orderables.ForEach(UpdateOrder);
        }

        private void UpdateOrder(IOrderable orderable)
        {
            foreach (var dep in orderable.GetDependencies())
            {
                if (dep.Order <= orderable.Order)
                {
                    dep.Order = orderable.Order + 1;
                    UpdateOrder(dep);
                }
            }
        }

        private List<IOrderable> orderables;
    }
}
