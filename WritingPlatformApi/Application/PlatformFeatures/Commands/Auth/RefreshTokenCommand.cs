using Contracts.Response.Auth;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.PlatformFeatures.Commands.Auth
{
    public class RefreshTokenCommand : IRequest<TokenApiResponse>
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    public class RefreshTokenCommandHandler : TokenApiBase, IRequestHandler<RefreshTokenCommand, TokenApiResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration configuration) : base(configuration)
        {
            _userManager = userManager;
        }

        public async Task<TokenApiResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default

            var user = await _userManager.FindByNameAsync(username) ?? throw new Exception("Invalid username");

            if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new Exception("Invalid client request");

            var newAccessToken = GenerateToken(principal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            await _userManager.UpdateAsync(user);

            return new TokenApiResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }
}


