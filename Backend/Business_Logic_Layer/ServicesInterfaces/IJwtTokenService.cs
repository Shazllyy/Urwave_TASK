using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.ServicesInterfaces
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user);
    }
}
