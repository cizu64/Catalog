using Catalog.Auth.Extensions;
using Catalog.Auth.Infrastructure.Repository;
using Catalog.Auth.Model;
using Catalog.Auth.Services;
using Catalog.Auth.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Catalog.Auth.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuth _auth;
        private readonly IUnitOfWork uow;

        public AuthController(ILogger<AuthController> logger, IAuth auth, IUnitOfWork uow)
        {
            _logger = logger;
            _auth = auth;
            this.uow = uow;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var login = await _auth.Authenticate(model.Email, model.Password);
            if (login is null)
            {
                return BadRequest(new
                {
                    Succeeded = false,
                    Message = "User not found"
                });
            }
            return Ok(new
            {
                Result = login,
                Succeeded = true,
                Message = "User logged in successfully"
            });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {

            await uow.Repository<User>().Add(new User
            {
                Email = model.Email,
                Fullname = model.Fullname,
                Password = model.Password.Hash(),

            });
            await uow.SaveChangesAsync();

            return Ok(new
            {
                Succeeded = true,
                Message = "User created successfully"
            });
        }       
    }
}