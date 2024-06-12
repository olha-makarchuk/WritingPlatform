using Application.PlatformFeatures.Queries.SortByItemQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.SortByItemQueriesTests
{
    public class GetAllSortByItemQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public GetAllSortByItemQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task GetAllSortByItemQuery_ShouldReturnSortByItems()
        {
            // Arrange
            var fixture = new Fixture();
            var sortByItems = fixture.CreateMany<SortByItem>().ToList();

            _dbContext.AddAndSaveRange(sortByItems);

            var query = new GetAllSortByItemQuery();

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(sortByItems.Count, result.Count());
            });
        }

        [Fact]
        public async Task GetAllSortByItemQuery_NoSortByItems_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new GetAllSortByItemQuery();

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

        private static GetAllSortByItemQuery.GetAllSortByItemQueryHandler CreateSut(ApplicationDbContext context)
            => new GetAllSortByItemQuery.GetAllSortByItemQueryHandler(context);
    }
}
