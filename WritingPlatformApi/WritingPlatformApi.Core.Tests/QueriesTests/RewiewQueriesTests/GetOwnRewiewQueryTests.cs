using Application.PlatformFeatures.Queries.Rewiew;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.RewiewQueriesTests
{
    public class GetOwnRewiewQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetOwnRewiewQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetOwnRewiewQuery_WithValidParameters_ShouldReturnRewiew()
        {
            // Arrange
            var fixture = new Fixture();
            var user = fixture.Build<ApplicationUser>().Create();
            var publication = fixture.Build<Publication>().Create();
            var rewiew = fixture.Build<UserRewiew>()
                                .With(r => r.ApplicationUserId, user.Id)
                                .With(r => r.PublicationId, publication.Id)
                                .Create();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(publication);
            _dbContext.AddAndSave(rewiew);

            var query = new GetOwnRewiewQuery { UserId = user.Id, PublicationId = publication.Id };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(rewiew.Id, result.Id);
                Assert.Equal(rewiew.ApplicationUserId, result.ApplicationUserId);
                Assert.Equal(rewiew.PublicationId, result.PublicationId);
            });
        }

        [Fact]
        public async Task GetOwnRewiewQuery_WithInvalidPublicationId_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();
            var user = fixture.Build<ApplicationUser>().Create();

            _dbContext.AddAndSave(user);

            var query = new GetOwnRewiewQuery { UserId = user.Id, PublicationId = 999 }; // Invalid PublicationId

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task GetOwnRewiewQuery_WithInvalidUserId_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();
            var publication = fixture.Build<Publication>().Create();

            _dbContext.AddAndSave(publication);

            var query = new GetOwnRewiewQuery { UserId = "invalidUserId", PublicationId = publication.Id }; // Invalid UserId

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task GetOwnRewiewQuery_WithNoRewiew_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();
            var user = fixture.Build<ApplicationUser>().Create();
            var publication = fixture.Build<Publication>().Create();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(publication);

            var query = new GetOwnRewiewQuery { UserId = user.Id, PublicationId = publication.Id };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        private GetOwnRewiewQuery.GetOwnRewiewQueryHandler CreateSut(ApplicationDbContext context)
            => new GetOwnRewiewQuery.GetOwnRewiewQueryHandler(context);
    }
}
