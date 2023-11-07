using Application.Models;
using Identity.API.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private string _rabbitUri = "rabbitmq://localhost/identityQueue";
        private readonly IServiceProvider _serviceProvider;

        public AccountController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [Authorize(Roles = "Editor")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("23423423");
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest request, CancellationToken token)
        {
            try
            {
                var response = await GetResponseRabbitTask<AuthenticateRequest, AuthenticateResponse>(request, token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken token)
        {
            try
            {
                var response = await GetResponseRabbitTask<RegisterRequest, RegisterResponse>(request, token);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }




        }


        private async Task<TOut> GetResponseRabbitTask<TIn, TOut>(TIn request, CancellationToken token)
            where TIn : class
            where TOut : class

        {
            var clientFactory = _serviceProvider.GetRequiredService<IClientFactory>();
            var client = clientFactory.CreateRequestClient<TIn>(new Uri(_rabbitUri));
            var response = await client.GetResponse<TOut>(request, token);
            return response.Message;
        }
    }
}
