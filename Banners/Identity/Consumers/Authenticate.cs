using Application.Models;
using Application.Services;
using MassTransit;

namespace Identity.API.Consumers
{
    public class Authenticate : IdentityBaseConsumer<Authenticate>, IConsumer<AuthenticateRequest>
    {
        public Authenticate(IUserService userService, ILogger<Authenticate> logger) : base(userService, logger)
        {
        }

        public async Task Consume(ConsumeContext<AuthenticateRequest> context)
        {

            AuthenticateResponse response = await _userService.Authenticate(context.Message);
            await context.RespondAsync<AuthenticateResponse>(response);
        }


    }
}
