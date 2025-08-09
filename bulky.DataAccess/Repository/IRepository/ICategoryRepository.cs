using bulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{
    //  Interface for managing Category entities,
    // extends the generic IRepository to include Category specific methods
    public interface ICategoryRepository : IRepository<Category>
    {
        //If I Want To Add Signature For Category Only
        // Update Category Details 

        void Update(Category category);
        
    }
}
