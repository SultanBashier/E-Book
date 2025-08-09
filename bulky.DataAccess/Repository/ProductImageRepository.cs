using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext db;

        //This Constructor For Injects the ApplicationDbContext and passes it to the base Repository class
        public ProductImageRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            db=dbContext;
        }

        // Updates an existing Product-image entity in the database

        public void Update(ProductImage obj)
        {
            db.productImages.Update(obj);

        }
    }
}
