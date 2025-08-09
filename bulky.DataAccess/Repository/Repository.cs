using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository
{

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;

    private DbSet<T> dbSet;

        //This Constructor For Injects the ApplicationDbContext and passes it to the base Repository class
        public Repository(ApplicationDbContext dbContext)
        {
            _db = dbContext;
            this.dbSet = _db.Set<T>();

            
            _db.products.Include(ca => ca.Category).Include(ca => ca.CategoryId);

        }

        // Adds a new entity to the DbSet
        public void Add(T entity)
        {
           // _db.Add(entity);
            dbSet.Add(entity);

        }

        // Returns all entities that match the optional filter
        // Supports eager loading for related entities using includeProperties
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Include navigation properties  if specified
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        // Returns a single entity matching the filter
        // Supports eager loading and tracking options
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query= dbSet;
            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }
            query =query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }
            }
            return query.FirstOrDefault();
          
         // return _db.Set<T>().Where(id); if i return prameter primitve 

        }

        // Removes a single entity from the DbSet
        public void Remove(T entity)
        {
            //_db.Remove(entity);
            dbSet.Remove(entity);
        }

        // Removes multiple entities from the DbSet
        public void RemoveRange(IEnumerable<T> entitiy)
        {
           // _db.RemoveRange(entitiy);
           dbSet.RemoveRange(entitiy);
        }
    }
}
