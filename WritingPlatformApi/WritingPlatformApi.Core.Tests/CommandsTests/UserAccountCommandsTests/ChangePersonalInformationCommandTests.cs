using Application.PlatformFeatures.Commands.UserAccountCommands;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.UserAccountCommandsTests
{
    public class ChangePersonalInformationCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly UserManagerDecorator _userManagerMock;
        private readonly CancellationTokenSource _cts = new();

        public ChangePersonalInformationCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = new UserManagerDecorator();
        }

        [Fact]
        public async Task Handle_UpdateUserInformation_ShouldUpdateUser()
        {
            // Arrange
            var fixture = new Fixture();

            var existingUser = fixture.Build<ApplicationUser>().Create();

            _dbContext.AddAndSave(existingUser);

            var command = new ChangePersonalInformationCommand
            {
                UserId = existingUser.Id,
                FirstName = "Jane",
                LastName = "Smith",
                PersonalInformation = "New personal information",
                UserName = "new_username"
            };

            _userManagerMock.SetupFindByIdAsync(existingUser);
            _userManagerMock.SetupFindByNameAsync(null); // Mocking that the new username doesn't exist

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.UserManagerMock.Object);

                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(command.UserId, result.Id);
                Assert.Equal(command.FirstName, result.FirstName);
                Assert.Equal(command.LastName, result.LastName);
                Assert.Equal(command.PersonalInformation, result.PersonalInformation);
                Assert.Equal(command.UserName, result.UserName);
            });
        }

        [Fact]
        public async Task Handle_UsernameAlreadyExists_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var existingUser = fixture.Build<ApplicationUser>().Create();

            var userWithSameUsername = fixture.Build<ApplicationUser>()
                                      .With(p => p.UserName, existingUser.UserName)
                                      .Create();

            _dbContext.AddAndSave(existingUser);
            _dbContext.AddAndSave(userWithSameUsername); // Add a user with the same username

            var command = new ChangePersonalInformationCommand
            {
                UserId = existingUser.Id,
                FirstName = "Jane",
                LastName = "Smith",
                PersonalInformation = "New personal information",
                UserName = "new_username" // Attempting to update with existing username
            };

            _userManagerMock.SetupFindByIdAsync(existingUser);
            _userManagerMock.SetupFindByNameAsync(userWithSameUsername); // Mocking that the new username already exists

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.UserManagerMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, _cts.Token));
                Assert.Equal("User with the same username alredy exists", exception.Message);
            });
        }

        private static UpdateGenreCommandHandler CreateSut(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            => new UpdateGenreCommandHandler(context, userManager);
    }
}