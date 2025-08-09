using bulkyBook.Models;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{//  Interface for managing Product Image entities,
    // extends the generic IRepository to include Product Image specific methods
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        //If I Want To Add Signature For Product Image Only
        // Update Product Image Details 
        void Update(ProductImage obj);
        
    }
}
