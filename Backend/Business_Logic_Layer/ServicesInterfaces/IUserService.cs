using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.ServicesInterfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(User user);
        Task<User> GetUserWithRoleAsync(Guid userId);
        Task<User> GetUserWithRoleByUserNameAsync(string username);  // New method

        Task<bool> UserExistsAsync(string username);

    }
}
