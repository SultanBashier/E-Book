using bulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{//  Interface for managing Order Header entities,
    // extends the generic IRepository to include Order Header specific methods
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        //If I Want To Add Signature For Order Header Only
        // Update Order Header Details 
        void Update(OrderHeader obj);

        void UpdateStatus(int Id, string OrderStatus, string? PaymentStatus=null);
        void UpdateStripePaymentId(int Id, string SessionId, string PaymentIntetId);
        
    }
}
