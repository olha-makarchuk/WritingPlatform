using Application.PlatformFeatures.Commands.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Application.Tests.PlatformFeatures.Commands.Auth;

namespace WritingPlatformApi.Core.Tests.Commands.Auth
{
    public class LoginCommandTests
    {
        private readonly UserManagerDecorator _userManagerMock;
        private readonly IConfiguration _configuration;
        private readonly LoginCommandHandler _handler;

        public LoginCommandTests()
        {
            _userManagerMock = new UserManagerDecorator();

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_userManagerMock.MyConfiguration)
                .Build();

            _handler = new LoginCommandHandler(_userManagerMock.UserManagerMock.Object, _configuration);
        }

        [Fact]
        public async Task Handle_ValidUserAndPassword_ReturnsLoginResponse()
        {
            // Arrange
            var command = new LoginCommand { username = "testuser", password = "Password123" };
            var user = new ApplicationUser { UserName = "testuser", Id = "1", IsActive = true };
            var userRoles = new List<string> { "User" };

            _userManagerMock.SetupFindByNameAsync(user);
            _userManagerMock.SetupCheckPasswordAsync(true);
            _userManagerMock.SetupGetRolesAsync(userRoles);
            _userManagerMock.SetupUpdateAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(user.UserName, result.UserName);
            Assert.NotNull(result.TokenApiResponse.AccessToken);
            Assert.NotNull(result.TokenApiResponse.RefreshToken);
        }

        [Fact]
        public async Task Handle_InvalidUsername_ThrowsException()
        {
            // Arrange
            var command = new LoginCommand { username = "invaliduser", password = "Password123" };

            _userManagerMock.SetupFindByNameAsync((ApplicationUser)null!);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InvalidPassword_ThrowsException()
        {
            // Arrange
            var command = new LoginCommand { username = "testuser", password = "InvalidPassword" };
            var user = new ApplicationUser { UserName = "testuser", Id = "1", IsActive = true };

            _userManagerMock.SetupFindByNameAsync(user);
            _userManagerMock.SetupCheckPasswordAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_InactiveUser_ThrowsException()
        {
            // Arrange
            var command = new LoginCommand { username = "inactiveuser", password = "Password123" };
            var user = new ApplicationUser { UserName = "inactiveuser", Id = "2", IsActive = false };

            _userManagerMock.SetupFindByNameAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
