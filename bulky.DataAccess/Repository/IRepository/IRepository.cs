using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.Repository.IRepository
{
    //  Generic repository interface for basic CRUD operations
 // Can be reused across different entity types (e.g., Product, Category, User)
    public interface IRepository<T> where T : class
    {

        //this all signature for all topic interface
        /// Retrieves a list of all entities from the database, with optional filtering and eager-loading.
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null,string? includeProperties = null);


        /// Retrieves a single entity matching the given filter, with optional eager-loading and tracking.
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null,bool tracked=false);

        /// Adds a new entity to the context.
        void Add(T entity);


        /// Removes a single entity from the context.
        void Remove(T entity);


        /// Removes a list of entities from the context.
        void RemoveRange(IEnumerable<T> entitiy);


    }
}
