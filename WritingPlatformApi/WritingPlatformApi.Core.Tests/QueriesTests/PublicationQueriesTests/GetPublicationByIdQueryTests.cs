using Application.PlatformFeatures.Queries.PublicationQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.PublicationQueriesTests
{
    public class GetPublicationByIdQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetPublicationByIdQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetPublicationByIdQuery_WithValidId_ShouldReturnPublication()
        {
            // Arrange
            var fixture = new Fixture();
            var publication = fixture.Build<Publication>()
                                     .With(p => p.Genre, fixture.Build<Genre>().Create())
                                     .With(p => p.ApplicationUser, fixture.Build<ApplicationUser>().Create())
                                     .Create();

            _dbContext.AddAndSave(publication);

            var query = new GetPublicationByIdQuery { IdPublication = publication.Id, PageNumber = 1, PageSize = 1 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(publication.Id, result.PublicationId);
            });
        }

        [Fact]
        public async Task GetPublicationByIdQuery_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            var query = new GetPublicationByIdQuery { IdPublication = -1, PageNumber = 1, PageSize = 1 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task GetPublicationByIdQuery_WithValidIdButNoMatchingPublication_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();
            var publication = fixture.Build<Publication>()
                                     .With(p => p.Genre, fixture.Build<Genre>().Create())
                                     .With(p => p.ApplicationUser, fixture.Build<ApplicationUser>().Create())
                                     .Create();

            _dbContext.AddAndSave(publication);

            var query = new GetPublicationByIdQuery { IdPublication = publication.Id + 1, PageNumber = 1, PageSize = 1 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        private GetPublicationByIdQuery.GetPublicationByIdQueryHandler CreateSut(ApplicationDbContext context)
            => new GetPublicationByIdQuery.GetPublicationByIdQueryHandler(context);
    }
}
