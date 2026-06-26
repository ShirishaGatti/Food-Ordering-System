using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListModel.Model
{
    class OrderItem
    {
        public int OrderItemId { get; set; }
        public Order orderId { get; set; }
        public Item ItemId { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public DbAdmin DbAdmin { get; set; }
    }
}
