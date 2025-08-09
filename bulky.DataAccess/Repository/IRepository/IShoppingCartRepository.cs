using bulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{//  Interface for managing ShoppingCart entities,
    // extends the generic IRepository to include ShoopingCart specific methods
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        //if i want to add signature for ShoppingCart only
        //Update ShoppingCart Details
        void Update(ShoppingCart obj);
        
    }
}
