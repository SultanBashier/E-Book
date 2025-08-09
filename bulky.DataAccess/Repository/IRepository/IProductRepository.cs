using bulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{//  Interface for managing Product entities,
    // extends the generic IRepository to include Product specific methods
    public interface IProductRepository : IRepository<Product>
    {
        //If I Want To Add Signature For Product Only
        // Update Product Details 
        void Update(Product product);

    }
}
