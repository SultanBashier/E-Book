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
    public class ApplicationUserRepository : Repository<ApllicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext db;


        //This Constructor For Injects the ApplicationDbContext and passes it to the base Repository class
        public ApplicationUserRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            db=dbContext;
        }

        // Updates an existing ApplicationUser entity in the database
        public void Update(ApllicationUser Obj)
        {
           db.apllicationUsers.Update(Obj);
        }
    }
}
