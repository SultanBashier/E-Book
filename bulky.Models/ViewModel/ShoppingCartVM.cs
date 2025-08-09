using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.Models.ViewModel
{
    public class ShoppingCartVM
    {

        // List of shopping cart items for the current user
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }


        // Contains order summary and user shipping/payment information
        public OrderHeader OrderHeader { get; set; }
    }
}
