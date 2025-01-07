using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserWithRoleAsync(Guid userId);
        Task<bool> UserExistsAsync(string username);
        Task CreateUserAsync(User user);
        Task<User> GetUserWithRoleByUserNameAsync(string username);  // New method

    }
}
