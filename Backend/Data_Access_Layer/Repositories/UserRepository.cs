using Data_Access_Layer.ApplicationDbContext;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbcontext _context;

        public UserRepository(ApplicationDbcontext context)
        {
            _context = context;
        }
        public async Task<User> GetUserWithRoleByUserNameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)  // Ensure the Role is loaded with the user
                .FirstOrDefaultAsync(u => u.UserName == username);
        }


        public async Task<User> GetUserWithRoleAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Role)  // Include Role data
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }

        public async Task CreateUserAsync(User user)
        {
            try
            {
                // Add the user to the database
                await _context.Users.AddAsync(user);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework or custom logging here)
                Console.WriteLine($"Error creating user: {ex.Message}");

                // Optionally, you can throw the exception again or handle it based on your business logic
                // For example, throw a custom exception or rethrow the original exception
                throw new ApplicationException("An error occurred while creating the user.", ex);
            }
        }

    }
}
