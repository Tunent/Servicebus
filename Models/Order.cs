using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order 
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Text { get; set; }
        public string OrderId { get; set; }
        public bool? Placed { get; set; }
        public bool? Billed { get; set; }
        public bool? Shipped { get; set; }

    }
}
