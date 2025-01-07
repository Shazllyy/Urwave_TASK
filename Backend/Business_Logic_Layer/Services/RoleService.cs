using Business_Logic_Layer.ServicesInterfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc/>
        public async Task AddRoleAsync(Role role)
        {
            // Add the role using the repository
            await _unitOfWork.RoleRepository.AddAsync(role);
            await _unitOfWork.SaveAsync();
        }

        /// <inheritdoc/>
        public async Task<Role> GetRoleByIdAsync(Guid roleId)
        {
            return await _unitOfWork.RoleRepository.GetByIdAsync(roleId);
        }

        /// <inheritdoc/>
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            var role = await _unitOfWork.RoleRepository.GetByNameAsync(roleName);
            return role != null;
        }

      
    }
}
