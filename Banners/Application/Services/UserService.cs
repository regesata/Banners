using Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<ApplicationUser> userManager, IConfiguration config,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }
        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await ValidateUser(user, model.Password))
            {
                throw new Exception("Invalid login or password");
            }
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id),

            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GenerateJwtToken(authClaims);
            return new AuthenticateResponse
            {
                Id = user.Id,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

        }

        public Task<RoleResponse> GetUserRole(RoleRequest model)
        {
            throw new NotImplementedException();
        }

        public async Task<RegisterResponse> Register(RegisterRequest model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null) { throw new Exception("User already exists"); }

            ApplicationUser user = new()
            {
                Email = model.Email,
                UserName = model.Email
            };

            await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, model.Role);
            user = await _userManager.FindByEmailAsync(model.Email);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.Role, model.Role)
            };



            var token = GenerateJwtToken(authClaims);

            return new RegisterResponse
            {
                Id = user.Id,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };



        }
        private async Task<bool> ValidateUser(ApplicationUser user, string password) =>

            await _userManager.CheckPasswordAsync(user, password);


        private JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:SecretKey"]));
            var token = new JwtSecurityToken(

                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.Now.AddHours(Int32.Parse(_config["Jwt:LifeTime"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)

            );
            return token;

        }
    }
}
