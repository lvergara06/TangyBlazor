using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangy_Models
{
    public class OrderDTO
    {
        public OrderHdrDTO OrderHdr { get; set; }
        public List<OrderDtlDTO> OrderDtls { get; set; }
    }
}
