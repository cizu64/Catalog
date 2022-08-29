using Catalog.Auth.Extensions;
using Catalog.Auth.Infrastructure.Repository;
using Catalog.Auth.Model;
using Catalog.Auth.Services;
using Catalog.Auth.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Security.Claims;
using System.Text;

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
            var user = new User
            {
                Email = model.Email,
                Fullname = model.Fullname,
                Password = model.Password.Hash()
            };
            await uow.Repository<User>().Add(user);
            await uow.SaveChangesAsync();

            //var Id = uow.Repository<User>().Entry(user).GetDatabaseValues().GetValue<int>("Id");

            //instead of publishing directly, use outbox pattern to store the message to the IntegrationEvent table in order to avoid data loss
            var integrationEventData = JsonConvert.SerializeObject(new
            {
                UserId = user.Id
            });
            await uow.Repository<IntegrationEvent>().Add(new()
            {
                Queue = "user.add",
                Data = integrationEventData
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