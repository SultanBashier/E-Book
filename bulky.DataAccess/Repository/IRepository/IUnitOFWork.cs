using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{
    //This Like a Container Include All Interfacs
    public interface IUnitOFWork
    {

        // Provides access to category-related data operations
        ICategoryRepository Category { get; }


        // Provides access to product-related data operations
        IProductRepository Product { get; }


        // Provides access to company-related data operations
        ICompanyRepository Company { get; }


        // Provides access to shopping cart-related operations
        IShoppingCartRepository ShoppingCart { get; }


        // Provides access to application user-related operations
        IApplicationUserRepository ApplicationUser { get; }


        // Provides access to order detail-related data operations
        IOrderDetailRepository OrderDetail { get; }


        // Provides access to order header-related data operations
        IOrderHeaderRepository OrderHeader { get; }


        // Provides access to product image-related operations
        IProductImageRepository ProductImage { get; }


        // Saves all changes made in the current transaction to the database
        void Save();
    }
}
