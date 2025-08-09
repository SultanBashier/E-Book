using bulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{
    //  Interface for managing ApplicationUser entities,
    // extends the generic IRepository to include user specific methods
    public interface IApplicationUserRepository : IRepository<ApllicationUser>
    {
        //  Update user details 
        void Update(ApllicationUser Obj);
    }
}
