using Application.Services;

namespace Identity.API.Consumers
{
    public class IdentityBaseConsumer<T>
    {
        protected readonly IUserService _userService;
        protected readonly ILogger<T> Logger;

        public IdentityBaseConsumer(IUserService userService, ILogger<T> logger)
        {
            _userService = userService;
            Logger = logger;
        }
    }
}
