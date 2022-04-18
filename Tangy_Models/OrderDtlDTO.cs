using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tangy_DataAccess;

namespace Tangy_Models
{
    public class OrderDtlDTO
    {
        public int Id { get; set; }
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
        public string Size { get; set; }
        public string ProductName { get; set; }
    }
}
