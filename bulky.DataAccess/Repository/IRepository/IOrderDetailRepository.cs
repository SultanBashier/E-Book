using bulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{//  Interface for managing Order Details entities,
    // extends the generic IRepository to include Order Details specific methods
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        //If I Want To Add Signature For Order Details Only
        // Update Order Details Information 
        void Update(OrderDetail obj);
        
    }
}
