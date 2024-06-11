using Application.PlatformFeatures.Commands.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WritingPlatformApi.Core.Tests.Commands.Auth
{
    public class RefreshTokenCommandTests
    {
        private readonly UserManagerDecorator _userManagerMock;
        private readonly IConfiguration _configuration;
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandTests()
        {
            _userManagerMock = new UserManagerDecorator();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_userManagerMock.MyConfiguration)
                .Build();

            _handler = new RefreshTokenCommandHandler(_userManagerMock.UserManagerMock.Object, _configuration);
        }

        [Fact]
        public async Task Handle_ValidRefreshToken_ReturnsNewTokens()
        {
            // Arrange
            var command = new RefreshTokenCommand { AccessToken = GenerateExpiredAccessToken(), RefreshToken = "valid_refresh_token" };
            var user = new ApplicationUser { UserName = "testuser", Id = "1", RefreshToken = "valid_refresh_token", RefreshTokenExpiryTime = DateTime.Now.AddDays(1) };

            _userManagerMock.SetupFindByNameAsync(user);
            _userManagerMock.SetupUpdateAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            Assert.NotEqual(command.RefreshToken, result.RefreshToken);
        }

        [Fact]
        public async Task Handle_InvalidRefreshToken_ThrowsException()
        {
            // Arrange
            var command = new RefreshTokenCommand { AccessToken = GenerateExpiredAccessToken(), RefreshToken = "invalid_refresh_token" };
            var user = new ApplicationUser { UserName = "testuser", Id = "1", RefreshToken = "valid_refresh_token", RefreshTokenExpiryTime = DateTime.Now.AddDays(1) };

            _userManagerMock.SetupFindByNameAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ExpiredRefreshToken_ThrowsException()
        {
            // Arrange
            var command = new RefreshTokenCommand { AccessToken = GenerateExpiredAccessToken(), RefreshToken = "valid_refresh_token" };
            var user = new ApplicationUser { UserName = "testuser", Id = "1", RefreshToken = "valid_refresh_token", RefreshTokenExpiryTime = DateTime.Now.AddDays(-1) };

            _userManagerMock.SetupFindByNameAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }


        private string GenerateExpiredAccessToken()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "testuser") 
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTKey:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWTKey:ValidIssuer"],
                audience: _configuration["JWTKey:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(-1), 
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
