using Application.PlatformFeatures.Commands.CommentCommands;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.CommentCommands
{
    public class CreateCommentTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();
        private readonly UserManagerDecorator _userManagerMock;

        public CreateCommentTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = new UserManagerDecorator();
        }

        [Fact]
        public async Task CreateCommentCommand_IfCommentCreates_ShouldReturnCreatedComment()
        {
            // Arrange
            var fixture = new Fixture();

            var user = fixture.Build<ApplicationUser>().Create();

            var publication = fixture.Build<Publication>().Create();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(publication);

            var commentCommand = fixture.Build<CreateCommentCommand>()
                .With(cc => cc.ApplicationUserId, user.Id)
                .With(cc => cc.PublicationId, publication.Id)
                .Create();

            _userManagerMock.SetupFindByIdAsync(user);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.UserManagerMock.Object);

                // Act
                var result = await handler.Handle(commentCommand, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(commentCommand.ApplicationUserId, result.ApplicationUserId);
                Assert.Equal(commentCommand.CommentText, result.CommentText);
                Assert.Equal(commentCommand.PublicationId, result.PublicationId);
            });
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var publication = fixture.Build<Publication>().Create();

            _dbContext.AddAndSave(publication);

            var commentCommand = fixture.Build<CreateCommentCommand>()
                .With(cc => cc.ApplicationUserId, "non-existing-user-id")
                .With(cc => cc.PublicationId, publication.Id)
                .Create();

            _userManagerMock.SetupFindByIdAsync(null);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.UserManagerMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(commentCommand, _cts.Token));
                Assert.Equal($"User with ID {commentCommand.ApplicationUserId} not found.", exception.Message);
            });
        }

        [Fact]
        public async Task Handle_WhenPublicationDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var user = fixture.Build<ApplicationUser>().Create();

            _dbContext.AddAndSave(user);

            var commentCommand = fixture.Build<CreateCommentCommand>()
                .With(cc => cc.ApplicationUserId, user.Id)
                .With(cc => cc.PublicationId, 999) 
                .Create();

            _userManagerMock.SetupFindByIdAsync(user);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.UserManagerMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(commentCommand, _cts.Token));
            });
        }

        private static CreateCommentCommandHandler CreateSut(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            => new(context, userManager);
    }
}
