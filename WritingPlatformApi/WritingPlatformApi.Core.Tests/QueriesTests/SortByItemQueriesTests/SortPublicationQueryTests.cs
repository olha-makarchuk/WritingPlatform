using Application.PlatformFeatures.Queries.SortByItemQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;
using static Application.PlatformFeatures.Queries.SortByItemQueries.SortPublicationQuery;

namespace WritingPlatformApi.Core.Tests.QueriesTests.SortByItemQueriesTests
{
    public class SortPublicationQueryTests
    {
       private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public SortPublicationQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task SortPublicationQuery_WithValidParameters_ShouldReturnSortedPublicationsByRating()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var user = fixture.Build<ApplicationUser>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .CreateMany(10)
                                      .ToList();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(genre);
            _dbContext.AddAndSaveRange(publications);

            var sortByItem = new SortByItem { Id = 1, FieldName = "Rating", ItemName = "Rating" };

            _dbContext.AddAndSave(sortByItem);

            var query = new SortPublicationQuery
            {
                GenreIds = new List<int> { genre.Id },
                StartPage = 0,
                EndPage = 1000,
                YearPublication = 0,
                SortBy = 1,
                SortDirection = "asc"
            };

            // Act
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10, result.Count);
            });
        }

        [Fact]
        public async Task SortPublicationQuery_WithValidParameters_ShouldReturnSortedPublicationsByDateAdding()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var user = fixture.Build<ApplicationUser>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .CreateMany(10)
                                      .ToList();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(genre);
            _dbContext.AddAndSaveRange(publications);

            var sortByItem = new SortByItem { Id = 1, FieldName = "DateAdding", ItemName = "DateAdding" };

            _dbContext.AddAndSave(sortByItem);

            var query = new SortPublicationQuery
            {
                GenreIds = new List<int> { genre.Id },
                StartPage = 0,
                EndPage = 1000,
                YearPublication = 0,
                SortBy = 1,
                SortDirection = "asc"
            };

            // Act
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10, result.Count);
            });
        }

        [Fact]
        public async Task SortPublicationQuery_WithValidParameters_ShouldReturnSortedPublicationsByNumberReviews()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var user = fixture.Build<ApplicationUser>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .CreateMany(10)
                                      .ToList();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(genre);
            _dbContext.AddAndSaveRange(publications);

            var sortByItem = new SortByItem { Id = 1, FieldName = "NumberReviews", ItemName = "NumberReviews" };

            _dbContext.AddAndSave(sortByItem);

            var query = new SortPublicationQuery
            {
                GenreIds = new List<int> { genre.Id },
                StartPage = 0,
                EndPage = 1000,
                YearPublication = 0,
                SortBy = 1,
                SortDirection = "asc"
            };

            // Act
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(10, result.Count);
            });
        }

        [Fact]
        public async Task SortPublicationQuery_WithInvalidSortBy_ShouldThrowException()
        {
            // Arrange
            var query = new SortPublicationQuery
            {
                GenreIds = new List<int>(),
                StartPage = 0,
                EndPage = 1000,
                YearPublication = 0,
                SortBy = 999, // Invalid SortBy ID
                SortDirection = "asc"
            };

            // Act and Assert
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task SortPublicationQuery_WithInvalidSortDirection_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var user = fixture.Build<ApplicationUser>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .CreateMany(10)
                                      .ToList();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(genre);
            _dbContext.AddAndSaveRange(publications);

            var sortByItem = new SortByItem { Id = 1, FieldName = "SomeItem", ItemName = "SomeItem" };
            _dbContext.AddAndSave(sortByItem);

            var query = new SortPublicationQuery
            {
                GenreIds = new List<int> { genre.Id },
                StartPage = 0,
                EndPage = 1000,
                YearPublication = 0,
                SortBy = 1,
                SortDirection = "invalid_direction"
            };

            // Act and Assert
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task SortPublicationQuery_WithGenreAndYear_ShouldReturnFilteredPublications()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var user = fixture.Build<ApplicationUser>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .With(p => p.DatePublication, new DateTime(2021, 1, 1))
                                      .CreateMany(5)
                                      .ToList();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(genre);
            _dbContext.AddAndSaveRange(publications);

            var sortByItem = new SortByItem { Id = 1, FieldName = "Rating", ItemName = "SomeItem" };
            _dbContext.AddAndSave(sortByItem);

            var query = new SortPublicationQuery
            {
                GenreIds = new List<int> { genre.Id },
                StartPage = 0,
                EndPage = 1000,
                YearPublication = 2021,
                SortBy = 1,
                SortDirection = "desc"
            };

            // Act
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(5, result.Count);
            });
        }

        [Fact]
        public async Task SortPublicationQuery_WithPageRange_ShouldReturnFilteredPublications()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var user = fixture.Build<ApplicationUser>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .With(p => p.CountOfPages, 150)
                                      .CreateMany(3)
                                      .Concat(fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .With(p => p.CountOfPages, 50)
                                      .CreateMany(2))
                                      .ToList();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(genre);
            _dbContext.AddAndSaveRange(publications);

            var sortByItem = new SortByItem { Id = 1, FieldName = "Rating", ItemName = "SomeItem" };
            _dbContext.AddAndSave(sortByItem);

            var query = new SortPublicationQuery
            {
                GenreIds = new List<int> { genre.Id },
                StartPage = 100,
                EndPage = 200,
                YearPublication = 0,
                SortBy = 1,
                SortDirection = "desc"
            };

            // Act
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(3, result.Count); // Only publications with page count between 100 and 200
            });
        }

        [Fact]
        public async Task SortPublicationQuery_WithMultipleCriteria_ShouldReturnFilteredPublications()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var user = fixture.Build<ApplicationUser>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .With(p => p.CountOfPages, 150)
                                      .With(p => p.DatePublication, new DateTime(2021, 1, 1))
                                      .CreateMany(2)
                                      .Concat(fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .With(p => p.CountOfPages, 50)
                                      .With(p => p.DatePublication, new DateTime(2020, 1, 1))
                                      .CreateMany(3))
                                      .ToList();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(genre);
            _dbContext.AddAndSaveRange(publications);

            var sortByItem = new SortByItem { Id = 1, FieldName = "Rating", ItemName = "SomeItem" };
            _dbContext.AddAndSave(sortByItem);

            var query = new SortPublicationQuery
            {
                GenreIds = new List<int> { genre.Id },
                StartPage = 100,
                EndPage = 200,
                YearPublication = 2021,
                SortBy = 1,
                SortDirection = "desc"
            };

            // Act
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count); // Only publications meeting all criteria
            });
        }

        [Fact]
        public async Task SortPublicationQuery_WithNoMatchingCriteria_ShouldReturnEmptyList()
        {
            // Arrange
            var fixture = new Fixture();
            var genre = fixture.Build<Genre>().Create();
            var user = fixture.Build<ApplicationUser>().Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.GenreId, genre.Id)
                                      .With(p => p.ApplicationUserId, user.Id)
                                      .CreateMany(5)
                                      .ToList();

            _dbContext.AddAndSave(user);
            _dbContext.AddAndSave(genre);
            _dbContext.AddAndSaveRange(publications);

            var sortByItem = new SortByItem { Id = 1, FieldName = "Rating", ItemName = "SomeItem" };
            _dbContext.AddAndSave(sortByItem);

            var query = new SortPublicationQuery
            {
                GenreIds = new List<int> { 9999 }, // Non-existing GenreId
                StartPage = 0,
                EndPage = 1000,
                YearPublication = 2021,
                SortBy = 1,
                SortDirection = "desc"
            };

            // Act
            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result); // No publications match the non-existing GenreId
            });
        }

        private SortPublicationQueryHandler CreateSut(ApplicationDbContext context)
            => new SortPublicationQueryHandler(context);
    }
}