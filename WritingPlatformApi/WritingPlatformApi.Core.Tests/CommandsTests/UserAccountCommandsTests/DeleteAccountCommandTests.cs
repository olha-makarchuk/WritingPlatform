using Application.PlatformFeatures.Commands.UserAccountCommands;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.UserAccountCommandsTests
{
    public class DeleteAccountCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly UserManagerDecorator _userManagerMock;
        private readonly CancellationTokenSource _cts = new();

        public DeleteAccountCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = new UserManagerDecorator();
        }

        [Fact]
        public async Task Handle_DeleteExistingUser_ShouldDeactivateUser()
        {
            // Arrange
            var fixture = new Fixture();

            var existingUser = fixture.Build<ApplicationUser>().Create();
            existingUser.IsActive = true;

            _dbContext.AddAndSave(existingUser);

            var command = new DeleteAccountCommand
            {
                UserId = existingUser.Id
            };

            _userManagerMock.SetupFindByIdAsync(existingUser);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.UserManagerMock.Object);

                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.False(result.IsActive); 
            });
        }

        [Fact]
        public async Task Handle_NonExistingUser_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var command = new DeleteAccountCommand
            {
                UserId = "non_existing_user_id"
            };

            _userManagerMock.SetupFindByIdAsync(null); 

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.UserManagerMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, _cts.Token));
                Assert.Equal("User not found", exception.Message);
            });
        }

        private static DeleteAccountCommandHandler CreateSut(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            => new DeleteAccountCommandHandler(context, userManager);
    }
}