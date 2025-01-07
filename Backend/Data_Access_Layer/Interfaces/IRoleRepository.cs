using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetByIdAsync(Guid roleId);

        Task<Role> GetByNameAsync(string roleName);

        Task CreateRoleAsync(Role role);

        Task UpdateRoleAsync(Role role);

        Task DeleteRoleAsync(Guid roleId);

        Task<IQueryable<Role>> GetAllRolesAsync();
        Task AddAsync(Role role);

    }
}
