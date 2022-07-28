using Catalog.Auth.Infrastructure.Repository;
using Catalog.Auth.Model;
using System.Security.Claims;
using Catalog.Auth.Extensions;
namespace Catalog.Auth.Services
{
    public interface IAuth
    {
        Task<string> Authenticate(string email, string password, bool hashPassword = true);
        int? getUserFromToken(string token);

        string? getRoleFromToken(string token);


    }
    public class Auth : IAuth
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuthenticate auth;
        private readonly IConfiguration _configuration;
        public Auth(IUnitOfWork uow, IAuthenticate auth, IConfiguration configuration)
        {
            _uow = uow;
            this.auth = auth;
            _configuration = configuration;
            this.auth.SecretKey = _configuration["jwt:secret"];
        }
        public async Task<string> Authenticate(string username, string password, bool hashPassword = true)
        {
            var hashPwd = hashPassword == true ? password.Hash() : password;
            var user = await _uow.Repository<User>().GetAsync(u => u.Email.ToLower() == username.ToLower() && u.Password == hashPwd);
            if (user is null) return null;
            var perform_auth = Authenticate(user.Id, user.Role);
            return perform_auth;
        }
        public int? getUserFromToken(string token)
        {

            var claims = auth.GetClaims(token.Replace("Bearer", string.Empty).Trim(), _configuration["jwt:issuer"], _configuration["jwt:audience"]);
            if (claims is not null)
            {
                string[] clms = claims.Select(x => x.Value).ToArray();
                var userId = clms[0];
                return int.Parse(userId);
            }
            return null;
        }
        public string? getRoleFromToken(string token)
        {

            var claims = auth.GetClaims(token.Replace("Bearer", string.Empty).Trim(), _configuration["jwt:issuer"], _configuration["jwt:audience"]);
            if (claims is not null)
            {
                string[] clms = claims.Select(x => x.Value).ToArray();
                string role = clms[1];
                return role;
            }
            return null;
        }
        string Authenticate(int userId, string role)
        {
            var claims = new ClaimsIdentity(new Claim[]
            {
                    new Claim(ClaimTypes.Name, userId.ToString()),
                    new Claim(ClaimTypes.Role, role),
            });
            var result = auth.CreateToken(claims, _configuration["jwt:issuer"], _configuration["jwt:audience"], 45);
            return result;
        }

       
    }
}
