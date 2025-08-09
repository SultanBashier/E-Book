using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.Models.ViewModel
{
    public class OrderVM
    {
        
        // Contains request header data such as user, date, total, status, title, etc.
        public OrderHeader orderHeader { get; set; }

        /// Order details list (each product within the order with quantity and price)
        public IEnumerable<OrderDetail> orderDetail { get; set; }
    }
}
