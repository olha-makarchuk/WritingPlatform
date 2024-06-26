using Microsoft.Extensions.Configuration;
using Domain.Entities;
using MediatR;
using System.IdentityModel.Tokens.Jwt;
using Contracts.Response.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Commands.Auth
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        [Required(ErrorMessage = "username is required.")]
        [MaxLength(256, ErrorMessage = "username cannot be longer than 256 characters.")]
        public string username { get; set; }

        [Required(ErrorMessage = "password is required.")]
        [MaxLength(256, ErrorMessage = "password cannot be longer than 256 characters.")]
        public string password { get; set; }
    }

    public class LoginCommandHandler : TokenApiBase, IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration configuration) : base(configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(request.username) ?? throw new Exception("Invalid username");

            if (!await _userManager.CheckPasswordAsync(user, request.password)) throw new Exception("Invalid password");

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateToken(authClaims);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(Convert.ToInt16(_configuration["JWTKey:RefreshTokenExpiryTimeInDay"]));

            await _userManager.UpdateAsync(user);

            return new LoginResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                TokenApiResponse = new TokenApiResponse
                {
                    AccessToken = token,
                    RefreshToken = refreshToken
                }
            };
        }
    }
}
