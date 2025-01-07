using Business_Logic_Layer.ServicesInterfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Uow;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public UserService(IUnitOfWork unitOfWork,  IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;

        }

        // Create User and assign Role
        public async Task<User> GetUserWithRoleByUserNameAsync(string username)
        {
            return await _unitOfWork.UserRepository.GetUserWithRoleByUserNameAsync(username);
        }
        public async Task CreateUserAsync(User user)
        {
            // Ensure the role exists or create a new one
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(user.RoleId);
            if (role == null)
            {
                throw new Exception("Role does not exist");
            }

            // Add the user to the User repository
            await _unitOfWork.UserRepository.CreateUserAsync(user);

            // Save changes in a single transaction
            await _unitOfWork.SaveAsync();
        }

        // Get User with their Role
        public async Task<User> GetUserWithRoleAsync(Guid userId)
        {
            return await _unitOfWork.UserRepository.GetUserWithRoleAsync(userId);
        }

        // Check if User exists
        public async Task<bool> UserExistsAsync(string username)
        {
            return await _unitOfWork.UserRepository.UserExistsAsync(username);
        }
    

    }
}
