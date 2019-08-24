
using System.Security.Claims;
using System.Threading.Tasks;

namespace VegankoService.Auth
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity, bool isAdmin);
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id);
    }
}
