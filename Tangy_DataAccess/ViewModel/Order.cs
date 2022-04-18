using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangy_DataAccess.ViewModel
{
    public class Order
    {
        public OrderHdr OrderHdr { get; set; }
        public IEnumerable<OrderDtl> OrderDtls { get; set; }
    }
}
