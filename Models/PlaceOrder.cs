using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class PlaceOrder
    {
        public string Id { get; set; }
        public DateTime OrderDate { get; set; }
        public Product Products { get; set; }

    }
}
