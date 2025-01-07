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
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbcontext _context;

        public RoleRepository(ApplicationDbcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(Role role)
        {
            // Check if a role with the same name already exists
            var existingRole = await _context.Roles
                                             .FirstOrDefaultAsync(r => r.Name == role.Name);

            if (existingRole != null)
            {
                throw new Exception($"A role with the name '{role.Name}' already exists.");
            }

            // Add the new role if it doesn't exist
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }
        public async Task<Role> GetByIdAsync(Guid roleId)
        {
            return await _context.Roles.FindAsync(roleId);
        }

        // Get a role by its name
        public async Task<Role> GetByNameAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }

        // Create a new role
        public async Task CreateRoleAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
        }

        // Update an existing role
        public async Task UpdateRoleAsync(Role role)
        {
            _context.Roles.Update(role);
        }

        // Delete a role
        public async Task DeleteRoleAsync(Guid roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role != null)
            {
                _context.Roles.Remove(role);
            }
        }

        // Get all roles (optional)
        public async Task<IQueryable<Role>> GetAllRolesAsync()
        {
            return _context.Roles.AsQueryable();
        }
    }
}
