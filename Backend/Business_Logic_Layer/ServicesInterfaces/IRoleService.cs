using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.ServicesInterfaces
{
    public interface IRoleService
    {
        Task AddRoleAsync(Role role);
        Task<Role> GetRoleByIdAsync(Guid roleId);
        Task<bool> RoleExistsAsync(string roleName);
    }

}
