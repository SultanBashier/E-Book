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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext db;

        //This Constructor For Injects the ApplicationDbContext and passes it to the base Repository class
        public ShoppingCartRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            db=dbContext;
        }

        // Updates an existing ShoppinfCart entity in the database
        public void Update(ShoppingCart obj)
        {
            db.shoppingCarts.Update(obj);

        }
    }
}
