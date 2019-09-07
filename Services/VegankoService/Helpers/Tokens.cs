

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VegankoService.Auth;
using VegankoService.Models;
using Newtonsoft.Json;

namespace VegankoService.Helpers
{
    public class Tokens
    {
      public static async Task<object> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions)
      {
        return new
        {
          id = identity.Claims.Single(c => c.Type == "id").Value,
          auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
          expires_in = (int)jwtOptions.ValidFor.TotalSeconds
        };
      }
    }
}
