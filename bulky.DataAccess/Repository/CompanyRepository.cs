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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;

        //This Constructor For Injects the ApplicationDbContext and passes it to the base Repository class
        public CompanyRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            db=dbContext;
        }

        // Updates an existing Company entity in the database

        public void Update(Company obj )
        {
            db.companies.Update(obj);

        }
    }
}
