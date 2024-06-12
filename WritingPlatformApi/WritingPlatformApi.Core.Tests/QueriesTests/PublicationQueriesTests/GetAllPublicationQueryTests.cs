using Application.PlatformFeatures.Queries.PublicationQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.PublicationQueriesTests
{
    public class GetAllPublicationQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetAllPublicationQueryTests()
            {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetAllPublicationQuery_WithValidParameters_ShouldReturnPublications()
        {
            // Arrange
            var fixture = new Fixture();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.Genre, fixture.Build<Genre>().Create())
                                      .With(p => p.ApplicationUser, fixture.Build<ApplicationUser>().Create())
                                      .CreateMany(10)
                                      .ToList();

            _dbContext.AddAndSaveRange(publications);

            var query = new GetAllPublicationQuery { PageNumber = 1, PageSize = 5 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(5, result.Count());
            });
        }

        [Fact]
        public async Task GetAllPublicationQuery_WithInvalidPageNumber_ShouldReturnEmptyList()
        {
            // Arrange
            var fixture = new Fixture();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.Genre, fixture.Build<Genre>().Create())
                                      .With(p => p.ApplicationUser, fixture.Build<ApplicationUser>().Create())
                                      .CreateMany(10)
                                      .ToList();

            _dbContext.AddAndSaveRange(publications);

            var query = new GetAllPublicationQuery { PageNumber = 3, PageSize = 5 }; // Only 2 pages of data

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            });
        }

        [Fact]
        public async Task GetAllPublicationQuery_WithInvalidPageSize_ShouldThrowException()
        {
            // Arrange
            var query = new GetAllPublicationQuery { PageNumber = 1, PageSize = 0 }; // Invalid PageSize

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        private static GetAllPublicationQuery.GetAllMoviesQueryHandler CreateSut(ApplicationDbContext context)
            => new GetAllPublicationQuery.GetAllMoviesQueryHandler(context);
    }
}
