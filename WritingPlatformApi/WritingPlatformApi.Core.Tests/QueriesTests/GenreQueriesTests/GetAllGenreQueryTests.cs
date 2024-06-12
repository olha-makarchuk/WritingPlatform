using Application.PlatformFeatures.Queries.GenreQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.GenreQueriesTests
{
    public class GetAllGenreQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetAllGenreQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetAllGenreQuery_WithExistingGenres_ShouldReturnGenres()
        {
            // Arrange
            var fixture = new Fixture();
            var genres = fixture.Build<Genre>().CreateMany().ToList();

            _dbContext.AddAndSaveRange(genres);

            var query = new GetAllGenreQuery();

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(genres.Count, result.Count());
            });
        }

        [Fact]
        public async Task GetAllGenreQuery_WithNoGenres_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new GetAllGenreQuery();

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

        private static GetAllGenreQuery.GetAllGenressQueryHandler CreateSut(ApplicationDbContext context)
            => new GetAllGenreQuery.GetAllGenressQueryHandler(context);
    }
}
