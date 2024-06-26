using Application.PlatformFeatures.Queries.AuthorQueries;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.AuthorQueriesTests
{
    public class GetAllAuthorQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        protected readonly CancellationTokenSource _cts = new();

        public GetAllAuthorQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null
            );
        }
        [Fact]
        public async Task GetAllAuthorQuery_WithValidParameters_ShouldReturnAuthors()
        {
            var query = new GetAllAuthorQuery { PageNumber = 1, PageSize = 10 };
            var fixture = new Fixture();
            var users = fixture.CreateMany<ApplicationUser>(10).ToList();
            var publications = users.Select(user => new Publication
            {
                ApplicationUserId = user.Id,
            }).ToList();

            _dbContext.AddAndSaveRange(users);
            _dbContext.AddAndSaveRange(publications);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.Object);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(users.Count, result.Count());
                foreach (var author in result)
                {
                    Assert.NotNull(author.IdAuthor);
                    Assert.NotNull(author.UserName);
                    Assert.NotNull(author.FirstName);
                    Assert.NotNull(author.LastName);
                    Assert.True(author.CountPublication > 0);
                }
            });
        }

        [Fact]
        public async Task GetAllAuthorQuery_WhenNoAuthors_ShouldReturnEmptyList()
        {
            // Arrange
            var query = new GetAllAuthorQuery { PageNumber = 1, PageSize = 10 };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _userManagerMock.Object);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            });
        }

        private static GetAllAuthorQuery.GetAllAuthorQueryHandler CreateSut(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
            => new(context, userManager);
    }
}
