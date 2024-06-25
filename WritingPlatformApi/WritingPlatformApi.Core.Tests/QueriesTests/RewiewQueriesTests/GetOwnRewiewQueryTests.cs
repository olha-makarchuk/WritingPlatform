using Application.PlatformFeatures.Queries.Review;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;
using Xunit;

namespace WritingPlatformApi.Core.Tests.QueriesTests.ReviewQueriesTests
{
    public class GetOwnReviewQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetOwnReviewQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetOwnReviewQuery_WithValidParameters_ShouldReturnReview()
        {
            // Arrange
            var fixture = new Fixture();
            var user = fixture.Build<ApplicationUser>().Create();
            var publication = fixture.Build<Publication>().Create();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(publication);

            var review = new UserRewiew()
            {
                ApplicationUserId = user.Id,
                PublicationId = publication.Id,
                Rewiew = 50
            };
            _dbContext.AddAndSave(review);

            var query = new GetOwnReviewQuery { UserId = user.Id, PublicationId = publication.Id };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(review.Id, result.Id);
                Assert.Equal(review.ApplicationUserId, result.ApplicationUserId);
                Assert.Equal(review.PublicationId, result.PublicationId);
            });
        }

        [Fact]
        public async Task GetOwnReviewQuery_WithInvalidPublicationId_ShouldThrowNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var user = fixture.Build<ApplicationUser>().Create();

            _dbContext.AddAndSave(user);

            var query = new GetOwnReviewQuery { UserId = user.Id, PublicationId = 999 }; // Invalid PublicationId

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task GetOwnReviewQuery_WithInvalidUserId_ShouldThrowNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var publication = fixture.Build<Publication>().Create();

            _dbContext.AddAndSave(publication);

            var query = new GetOwnReviewQuery { UserId = "invalidUserId", PublicationId = publication.Id }; // Invalid UserId

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task GetOwnReviewQuery_WithNoReview_ShouldThrowNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var user = fixture.Build<ApplicationUser>().Create();
            var publication = fixture.Build<Publication>().Create();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(publication);

            var query = new GetOwnReviewQuery { UserId = user.Id, PublicationId = publication.Id };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act & Assert
                await Assert.ThrowsAsync<NotFoundException>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        private GetOwnReviewQuery.GetOwnReviewQueryHandler CreateSut(ApplicationDbContext context)
            => new GetOwnReviewQuery.GetOwnReviewQueryHandler(context);
    }
}
