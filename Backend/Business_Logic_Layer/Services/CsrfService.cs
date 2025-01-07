using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class CsrfService
    {
        private readonly IAntiforgery _antiForgery;

        public CsrfService(IAntiforgery antiForgery)
        {
            _antiForgery = antiForgery;
        }

        public string GenerateCsrfToken(HttpContext context)
        {
            var tokens = _antiForgery.GetAndStoreTokens(context);
            return tokens.RequestToken;
        }
        public bool ValidateCsrfToken(HttpContext context)
        {
            try
            {
                _antiForgery.ValidateRequestAsync(context).Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
