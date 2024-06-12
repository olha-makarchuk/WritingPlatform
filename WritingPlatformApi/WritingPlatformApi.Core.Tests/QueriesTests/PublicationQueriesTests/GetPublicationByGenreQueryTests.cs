using Application.PlatformFeatures.Queries.PublicationQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.PublicationQueriesTests
{
    public class GetPublicationByGenreQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetPublicationByGenreQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetPublicationByGenreQuery_WithValidParameters_ShouldReturnPublications()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUser, fixture.Build<ApplicationUser>().Create())
                                      .CreateMany(10)
                                      .ToList();

            _dbContext.AddAndSaveRange(publications);
            _dbContext.AddAndSave(genre);

            var query = new GetPublicationByGenreQuery { IdGenre = genre.Id, PageNumber = 1, PageSize = 5 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(5, result.Count);
            });
        }

        [Fact]
        public async Task GetPublicationByGenreQuery_WithInvalidGenreId_ShouldThrowException()
        {
            // Arrange
            var query = new GetPublicationByGenreQuery { IdGenre = -1, PageNumber = 1, PageSize = 5 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task GetPublicationByGenreQuery_WithNoPublications_ShouldReturnEmptyList()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            _dbContext.AddAndSave(genre);

            var query = new GetPublicationByGenreQuery { IdGenre = genre.Id, PageNumber = 1, PageSize = 5 };

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

        private GetPublicationByGenreQuery.GetMovieByIdQueryHandler CreateSut(ApplicationDbContext context)
            => new GetPublicationByGenreQuery.GetMovieByIdQueryHandler(context);
    }
}
