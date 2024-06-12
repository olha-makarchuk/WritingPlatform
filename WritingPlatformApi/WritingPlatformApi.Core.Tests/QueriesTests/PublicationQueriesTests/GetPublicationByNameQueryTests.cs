using Application.PlatformFeatures.Queries.PublicationQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.PublicationQueriesTests
{
    public class GetPublicationByNameQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetPublicationByNameQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetPublicationByNameQuery_WithValidName_ShouldReturnPublications()
        {
            // Arrange
            var fixture = new Fixture();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.PublicationName, "Test Publication")
                                      .With(p => p.Genre, fixture.Build<Genre>().Create())
                                      .With(p => p.ApplicationUser, fixture.Build<ApplicationUser>().Create())
                                      .CreateMany()
                                      .ToList();

            _dbContext.AddAndSaveRange(publications);

            var query = new GetPublicationByNameQuery { PublicationName = "Test Publication", PageNumber = 1, PageSize = 10 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(publications.Count, result.Count);
            });
        }

        [Fact]
        public async Task GetPublicationByNameQuery_WithInvalidName_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new GetPublicationByNameQuery { PublicationName = "Nonexistent Publication", PageNumber = 1, PageSize = 10 };

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

        private GetPublicationByNameQuery.GetPublicationByNameQueryHandler CreateSut(ApplicationDbContext context)
            => new GetPublicationByNameQuery.GetPublicationByNameQueryHandler(context);
    }
}
