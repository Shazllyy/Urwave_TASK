using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using DataAccessLayer.Interfaces;
using System;
using System.Threading.Tasks;

namespace Data_Access_Layer.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Product { get; }
        ICategoryRepository Category { get; }
        IMetricsRepository MetricsRepository { get; }
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }

        Task<int> SaveAsync();
    }
}
