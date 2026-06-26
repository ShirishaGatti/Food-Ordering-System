using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemListModel.Model
{
    class Order
    {
        public int OrderId { get; set; }
	//public Date OrderDate { get; set; }
	public User userId { get; set; }
	    public DbAdmin DbAdmin { get; set; }
    }
}
