using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly DbContext _context;
        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public Dictionary<Type, object> Repositories = new Dictionary<Type, object>();

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IGenericRepository<T>;
            }
            IGenericRepository<T> repo = new GenericRepository<T>(_context);
            Repositories.Add(typeof(T), repo);
            return repo;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public DbContext GetContext
        {
            get { return _context; }
        }
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    }
}
