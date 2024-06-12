using Application.PlatformFeatures.Commands.RewiewCommand;
using AutoFixture;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.RewiewCommandsTests
{
    public class DeleteRewiewCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly UserManagerDecorator _userManagerMock;
        private readonly CancellationTokenSource _cts = new();

        public DeleteRewiewCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = new UserManagerDecorator();
        }

        [Fact]
        public async Task Handle_ExistingReview_ShouldDeleteReviewAndUpdatePublication()
        {
            // Arrange
            var fixture = new Fixture();

            var publication = new Publication
            {
                Id = 1,
                PublicationName = "Sample Publication",
                ApplicationUserId = "qwerty",
                bookDescription = "qwerty",
                CountOfPages = 2,
                FileKey = "FileKey",
                TitleKey = "Title",
                DatePublication = DateTime.Now,
                GenreId = 1,
                Rating = 3,
                CountOfRewiews = 2 // Example initial values
            };

            var userReviewToDelete = new UserRewiew
            {
                Id = 1,
                PublicationId = publication.Id,
                Rewiew = 4, // Review to delete
                ApplicationUserId = "sample-user-id"
            };

            _dbContext.AddAndSave(publication);
            _dbContext.AddAndSave(userReviewToDelete);

            var command = new DeleteRewiewCommand
            {
                RewiewId = userReviewToDelete.Id
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userReviewToDelete.Id, result.Id); // Ensure the returned review ID matches the deleted one

                // Verify publication's updated values
                var updatedPublication = await context.Publication.FirstOrDefaultAsync(p => p.Id == publication.Id);
                Assert.Equal(1, updatedPublication.CountOfRewiews); // Count of reviews should decrease by 1
                Assert.Equal(2, updatedPublication.Rating); // Assuming (3 * 2 - 4) / 1 = 2

                // Ensure the deleted review no longer exists in the database
                var deletedReview = await context.UserRewiew.FindAsync(userReviewToDelete.Id);
                Assert.Null(deletedReview);
            });
        }

        [Fact]
        public async Task Handle_NonExistingReview_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var command = new DeleteRewiewCommand
            {
                RewiewId = 999 // Non-existent review ID
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, _cts.Token));
            });
        }

        private static DeleteRewiewCommandHandler CreateSut(ApplicationDbContext context)
            => new DeleteRewiewCommandHandler(context);
    }
}

