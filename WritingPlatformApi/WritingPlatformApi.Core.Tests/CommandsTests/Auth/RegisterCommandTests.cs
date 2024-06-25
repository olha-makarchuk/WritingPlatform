using Application.PlatformFeatures.Commands.Auth;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using WritingPlatformApi.Core.Tests;
using Xunit;

namespace Application.Tests.PlatformFeatures.Commands.Auth
{
    public class RegistrationCommandHandlerTests
    {
        private readonly UserManagerDecorator _userManagerMockSetup;
        private readonly RoleManagerDecorator _roleManagerMockSetup;
        private readonly RegistrationCommandHandler _handler;

        public RegistrationCommandHandlerTests()
        {
            _userManagerMockSetup = new UserManagerDecorator();
            _roleManagerMockSetup = new RoleManagerDecorator();
            _handler = new RegistrationCommandHandler(
                _userManagerMockSetup.UserManagerMock.Object,
                _roleManagerMockSetup.RoleManagerMock.Object
            );
        }

        [Fact]
        public async Task Handle_UserRegistrationSuccess_ReturnsRegistrationResponse()
        {
            // Arrange
            var command = new RegistrationCommand
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            _userManagerMockSetup.SetupFindByNameAsyncTwice((ApplicationUser)null!, new ApplicationUser() { UserName = "testuser", Email = "testuser@example.com" });
            _userManagerMockSetup.SetupCreateAsync(IdentityResult.Success);
            _roleManagerMockSetup.SetupRoleExistsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.UserName, result.UserName);
            Assert.Equal(command.Email, result.Email);
        }

        [Fact]
        public async Task Handle_UserAlreadyExists_ThrowsException()
        {
            // Arrange
            var command = new RegistrationCommand
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            _userManagerMockSetup.SetupFindByNameAsync(new ApplicationUser());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_UserCreationFailed_ThrowsException()
        {
            // Arrange
            var command = new RegistrationCommand
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            _userManagerMockSetup.SetupFindByNameAsync(null);
            _userManagerMockSetup.SetupCreateAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed!" }));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
        
        [Fact]
        public async Task Handle_RoleCreationFailed_ThrowsException()
        {
            // Arrange
            var command = new RegistrationCommand
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            _userManagerMockSetup.SetupFindByNameAsync(null);
            _userManagerMockSetup.SetupCreateAsync(IdentityResult.Success);
            _roleManagerMockSetup.SetupRoleExistsAsync(false);
            _roleManagerMockSetup.SetupCreateAsync(IdentityResult.Failed(new IdentityError { Description = "Role creation failed!" }));

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
