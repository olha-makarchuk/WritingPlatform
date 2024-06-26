using Application.Interfaces;
using Application.PlatformFeatures.Commands.PublicationCommands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.PublicationCommands
{
    public class DeletePublicationCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly Mock<IBlobStorage> _storageMock;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly UserManagerDecorator _userManagerDecorator;

        public DeletePublicationCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _storageMock = new Mock<IBlobStorage>();
            _userManagerDecorator = new UserManagerDecorator();
        }

        [Fact]
        public async Task Handle_WhenPublicationExists_ShouldDeletePublicationAndAssociatedEntities()
        {
            // Arrange
            var publication = new Publication
            {
                Id = 1,
                FileKey = "file_key",
                TitleKey = "title_key",
                ApplicationUserId = "user_id"
            };
            _dbContext.AddAndSave(publication);

            var comments = new List<Comment>
        {
            new Comment { Id = 1, PublicationId = 1, ApplicationUserId = publication.ApplicationUserId, CommentText="smth"},
            new Comment { Id = 2, PublicationId = 1, ApplicationUserId = publication.ApplicationUserId, CommentText="smth"}
        };
            _dbContext.AddAndSaveRange(comments);

            var command = new DeletePublicationByIdCommand { Id = 1 };

            _storageMock.Setup(s => s.DeleteAsync(publication.FileKey)).Verifiable();
            _storageMock.Setup(s => s.DeleteAsync(publication.TitleKey)).Verifiable();

            var user = new ApplicationUser { Id = "user_id", UserName = "testuser", IsAuthor = true };
            _userManagerDecorator.SetupFindByIdAsync(user);
            _userManagerDecorator.SetupUpdateAsync(IdentityResult.Success);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object, _userManagerDecorator.UserManagerMock.Object);
                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(publication.Id, result.Id);
            });
        }

        [Fact]
        public async Task Handle_WhenPublicationNotFound_ShouldThrowException()
        {
            // Arrange
            var command = new DeletePublicationByIdCommand { Id = 1 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object, _userManagerDecorator.UserManagerMock.Object);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, _cts.Token));
            });
        }


        [Fact]
        public async Task Handle_WhenNoOtherPublicationsExistForUser_ShouldUpdateUserIsAuthorFalse()
        {
            // Arrange
            var publication = new Publication
            {
                Id = 1,
                FileKey = "file_key",
                TitleKey = "title_key",
                ApplicationUserId = "user_id"
            };
            _dbContext.AddAndSave(publication);

            var command = new DeletePublicationByIdCommand { Id = 1 };

            _storageMock.Setup(s => s.DeleteAsync(publication.FileKey)).Verifiable();
            _storageMock.Setup(s => s.DeleteAsync(publication.TitleKey)).Verifiable();

            var user = new ApplicationUser { Id = "user_id", UserName = "testuser", IsAuthor = true };
            _userManagerDecorator.SetupFindByIdAsync(user);
            _userManagerDecorator.SetupUpdateAsync(IdentityResult.Success);

            _dbContext.Assert(async context =>
            {
                var handler = new DeletePublicationCommandHandler(context, _userManagerDecorator.UserManagerMock.Object, _storageMock.Object);

                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(publication.Id, result.Id);

                _userManagerDecorator.UserManagerMock.Verify(u => u.FindByIdAsync(publication.ApplicationUserId), Times.Once);
                _userManagerDecorator.UserManagerMock.Verify(u => u.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
            });
        }

        private static DeletePublicationCommandHandler CreateSut(
            ApplicationDbContext context,
            IBlobStorage storage,
            UserManager<ApplicationUser> userManager)
            => new(context, userManager, storage);
    }
}
