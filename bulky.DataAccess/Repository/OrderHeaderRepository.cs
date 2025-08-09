using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext db;

        //This Constructor For Injects the ApplicationDbContext and passes it to the base Repository class
        public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            db = dbContext;
        }

        // Updates an existing Order-Header entity in the database

        public void Update(OrderHeader obj)
        {
            db.orderHeaders.Update(obj);

        }

        public void UpdateStatus(int Id, string OrderStatus, string? PaymentStatus = null)
        {
            var OrderFromDb = db.orderHeaders.FirstOrDefault(o => o.Id == Id);
            if (OrderFromDb != null)
            {
                OrderFromDb.OrderStatus = OrderStatus;
                if (!string.IsNullOrEmpty(PaymentStatus))
                {
                    OrderFromDb.PaymentStatus = PaymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int Id, string SessionId, string PaymentIntetId)
        {
            var OrderFromDb = db.orderHeaders.FirstOrDefault(p => p.Id == Id);
            if (!string.IsNullOrEmpty(SessionId))
            {
                OrderFromDb.SessionId = SessionId;
            }
            if (!string.IsNullOrEmpty(PaymentIntetId))
            {
                OrderFromDb.PaymentIntenId = PaymentIntetId;
                OrderFromDb.PaymentDate = DateTime.Now;

            }
        }
    }
}

