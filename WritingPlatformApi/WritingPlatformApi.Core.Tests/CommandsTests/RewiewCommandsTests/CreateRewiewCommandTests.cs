using Application.PlatformFeatures.Commands.RewiewCommand;
using AutoFixture;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.RewiewCommandsTests
{
    public class CreateRewiewCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly UserManagerDecorator _userManagerMock;
        private readonly CancellationTokenSource _cts = new();

        public CreateRewiewCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = new UserManagerDecorator();
        }

        [Fact]
        public async Task Handle_ValidRewiewCommand_ShouldCreateRewiewAndReturnUpdatedPublication()
        {
            // Arrange
            var fixture = new Fixture();

            var publication = new Publication
            {
                Id = 1,
                PublicationName = "Sample Publication",
                ApplicationUserId="qwerty",
                bookDescription="qwerty",
                CountOfPages=2,
                FileKey= "FileKey",
                TitleKey= "Title",
                DatePublication= DateTime.Now,
                GenreId=1,
                Rating = 4,
                CountOfRewiews = 2 // Example initial values
            };

            _dbContext.AddAndSave(publication);

            var command = new CreateRewiewCommand
            {
                IdPublication = publication.Id,
                Rating = 5,
                UserId = "sample-user-id"
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(publication.Id, result.Id);
                Assert.Equal(3, result.CountOfRewiews); // Assert the updated count of reviews
                Assert.Equal(4, result.Rating); // Assert the calculated average rating (assuming integer division in the handler)
            });
        }

        [Fact]
        public async Task Handle_PublicationNotFound_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var command = new CreateRewiewCommand
            {
                IdPublication = 999, // Non-existent publication ID
                Rating = 5,
                UserId = "sample-user-id"
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, _cts.Token));
            });
        }

        [Fact]
        public async Task Handle_FirstReviewForPublication_ShouldSetRatingEqualToCommandRating()
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
                Rating = 0,
                CountOfRewiews = 0 // Example initial values
            };

            _dbContext.AddAndSave(publication);

            var command = new CreateRewiewCommand
            {
                IdPublication = publication.Id,
                Rating = 5,
                UserId = "sample-user-id"
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(publication.Id, result.Id);
                Assert.Equal(1, result.CountOfRewiews); // Assert the updated count of reviews
                Assert.Equal(command.Rating, result.Rating); // Assert the rating equals the command's rating
            });
        }

        [Fact]
        public async Task Handle_UserHasAlreadyReviewed_ShouldThrowArgumentException()
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
                Rating = 4,
                CountOfRewiews = 1 // Example initial values
            };

            var existingReview = new UserRewiew
            {
                PublicationId = publication.Id,
                Rewiew = 5,
                ApplicationUserId = "sample-user-id"
            };

            _dbContext.AddAndSave(publication);
            _dbContext.AddAndSave(existingReview);

            var command = new CreateRewiewCommand
            {
                IdPublication = publication.Id,
                Rating = 5,
                UserId = "sample-user-id"
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, _cts.Token));
                Assert.Equal("User has already reviewed this publication.", exception.Message);
            });
        }
        private static CreateRewiewCommandHandler CreateSut(ApplicationDbContext context)
            => new CreateRewiewCommandHandler(context);
    }
}
