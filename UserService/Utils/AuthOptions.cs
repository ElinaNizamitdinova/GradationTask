using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UserService.Utils
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        const string KEY = "614efe1c-39e4-45b4-b121-3fab583780bf";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
