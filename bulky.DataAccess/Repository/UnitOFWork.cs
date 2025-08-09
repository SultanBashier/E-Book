using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository
{
    public class UnitOFWork : IUnitOFWork
    {
        private readonly ApplicationDbContext _db;


        // Access to category-specific data operations
        public ICategoryRepository Category { get; private set; }



        // Access to product-specific data operations
        public IProductRepository Product { get; private set; }



        // Access to company-specific data operations
        public ICompanyRepository Company { get; private set; }



        // Access to shopping cart-specific data operations
        public IShoppingCartRepository ShoppingCart { get; private set; }



        // Access to application user-specific data operations
        public IApplicationUserRepository ApplicationUser { get; private set; }



        // Access to order details-specific data operations
        public IOrderDetailRepository OrderDetail { get; private set; }



        // Access to order header-specific data operations
        public IOrderHeaderRepository OrderHeader { get; private set; }



        // Access to product image-specific data operations
        public IProductImageRepository ProductImage { get; private set; }



        //This  Constructor For  initialize all repositories with the same DB context
        public UnitOFWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            ProductImage = new ProductImageRepository(_db);
        }

        // Save all changes across repositories in a single transaction
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}