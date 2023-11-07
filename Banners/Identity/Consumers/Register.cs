using Application.Models;
using Application.Services;
using MassTransit;

namespace Identity.API.Consumers
{
    public class Register : IdentityBaseConsumer<Register>, IConsumer<RegisterRequest>
    {
        public Register(IUserService userService, ILogger<Register> logger) : base(userService, logger)
        {

        }

        public async Task Consume(ConsumeContext<RegisterRequest> context)
        {
            RegisterResponse response = await _userService.Register(context.Message);
            await context.RespondAsync<RegisterResponse>(response);
        }
    }
}
