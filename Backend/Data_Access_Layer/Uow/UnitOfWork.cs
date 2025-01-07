using Data_Access_Layer.ApplicationDbContext;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using Data_Access_Layer.Repositories;
using Data_Access_Layer.Uow;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using System;
using System.Threading.Tasks;

namespace Data_Access_Layer.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbcontext _context;

        // Repositories for Product and Category
        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }

        public IMetricsRepository MetricsRepository { get; }
        public IUserRepository UserRepository { get; }
       public IRoleRepository RoleRepository { get; }



        public UnitOfWork(ApplicationDbcontext context)
        {
            _context = context;
            Product = new ProductRepository(_context);
            Category = new CategoryRepository(_context);
            MetricsRepository = new MetricsRepository(_context);
            UserRepository = new UserRepository(_context);
            RoleRepository = new RoleRepository(_context);
        }

        // Method to save changes to the database
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose method to clean up the context
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
