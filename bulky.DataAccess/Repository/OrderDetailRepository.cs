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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext db;

        //This Constructor For Injects the ApplicationDbContext and passes it to the base Repository class
        public OrderDetailRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            db=dbContext;
        }

        // Updates an existing Order-Deatails entity in the database

        public void Update(OrderDetail obj)
        {
            db.orderDetails.Update(obj);

        }
    }
}
