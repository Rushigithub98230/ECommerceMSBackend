using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EComMSSharedLibrary.JwtTokenHandler
{
    public interface IJwtTokenHandler
    {
        string GenerateToken(string userId, string email, string role, IEnumerable<Claim>? additionalClaims = null);
        bool ValidateToken(string token, out ClaimsPrincipal? claimsPrincipal);
    }
}
