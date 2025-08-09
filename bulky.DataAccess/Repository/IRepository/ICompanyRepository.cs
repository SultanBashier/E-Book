using bulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{
    //  Interface for managing Company entities,
    // extends the generic IRepository to include Company specific methods
    public interface ICompanyRepository : IRepository<Company>
    {
        //If I Want To Add Signature For Company Only
        // Update Company Details 
        void Update(Company obj);
        
    }
}
