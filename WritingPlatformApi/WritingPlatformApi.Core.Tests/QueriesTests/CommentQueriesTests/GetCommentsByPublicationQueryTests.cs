using Application.PlatformFeatures.Queries.CommentQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.CommentQueriesTests
{
    public class GetCommentsByPublicationQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetCommentsByPublicationQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetCommentsByPublicationQuery_WithValidParameters_ShouldReturnComments()
        {
            // Arrange
            var fixture = new Fixture();
            var publications = fixture.Build<Publication>()
                                      .CreateMany()
                                      .ToList();

            var comments = fixture.Build<Comment>()
                                 .With(c => c.PublicationId, publications[0].Id)
                                 .CreateMany()
                                 .ToList();

            _dbContext.AddAndSaveRange(publications);
            _dbContext.AddAndSaveRange(comments);

            var query = new GetCommentsByPublicationQuery { IdPublication = publications[0].Id };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(comments.Count, result.Count);
            });
        }

        [Fact]
        public async Task GetCommentsByPublicationQuery_WithInvalidPublicationId_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();
            var publications = fixture.Build<Publication>().CreateMany().ToList();

            _dbContext.AddAndSaveRange(publications);

            var query = new GetCommentsByPublicationQuery { IdPublication = -1 }; 

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        private static GetCommentsByPublicationQuery.GetCommentByPublicationQuerieHandler CreateSut(ApplicationDbContext context)
            => new GetCommentsByPublicationQuery.GetCommentByPublicationQuerieHandler(context);
    }
}
