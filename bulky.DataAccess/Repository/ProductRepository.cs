using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository.IRepository;
using bulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {

        private readonly ApplicationDbContext db;


        //This Constructor For Injects the ApplicationDbContext and passes it to the base Repository class
        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            db = dbContext;
        }

        // Updates an existing Product entity in the database

        public void Update(Product product)
        {
            // db.products.Update(product);
            var objfromdb = db.products.FirstOrDefault(pd => pd.Id == product.Id);
            if(objfromdb!=null)
            {objfromdb.Title = product.Title;
                objfromdb.ISBN = product.ISBN;
                objfromdb.Price = product.Price;
                objfromdb.Price50 = product.Price50;
                objfromdb.Price100 = product.Price100;
                objfromdb.ListPrice = product.ListPrice;
                objfromdb.Description = product.Description;
                objfromdb.CategoryId = product.CategoryId;
                objfromdb.Author = product.Author;
                objfromdb.productImages = product.productImages;
               

            }
        }
    }
}
