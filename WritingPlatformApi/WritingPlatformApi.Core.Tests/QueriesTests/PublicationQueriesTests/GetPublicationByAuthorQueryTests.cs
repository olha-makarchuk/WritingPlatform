using Application.PlatformFeatures.Queries.PublicationQueries;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.PublicationQueriesTests
{
    public class GetPublicationByAuthorQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        protected readonly CancellationTokenSource _cts = new();

        public GetPublicationByAuthorQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = MockUserManager();
        }

        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task GetPublicationByAuthorQuery_WithValidParameters_ShouldReturnPublications()
        {
            // Arrange
            var fixture = new Fixture();
            var author = fixture.Build<ApplicationUser>().With(u => u.IsAuthor, true).Create();
            var publications = fixture.Build<Publication>()
                                      .With(p => p.ApplicationUserId, author.Id)
                                      .With(p => p.Genre, fixture.Build<Genre>().Create())
                                      .CreateMany(10)
                                      .ToList();

            _dbContext.AddAndSaveRange(publications);
            _dbContext.AddAndSave(author);

            _userManagerMock.Setup(um => um.FindByIdAsync(author.Id)).ReturnsAsync(author);

            var query = new GetPublicationByAuthorQuery { UserId = author.Id, PageNumber = 1, PageSize = 5 };

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
        public async Task GetPublicationByAuthorQuery_WithInvalidAuthorId_ShouldThrowException()
        {
            // Arrange
            var query = new GetPublicationByAuthorQuery { UserId = "invalidUserId", PageNumber = 1, PageSize = 5 };

            _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task GetPublicationByAuthorQuery_WithNoPublications_ShouldReturnEmptyList()
        {
            // Arrange
            var fixture = new Fixture();
            var author = fixture.Build<ApplicationUser>().With(u => u.IsAuthor, true).Create();
            _dbContext.AddAndSave(author);

            _userManagerMock.Setup(um => um.FindByIdAsync(author.Id)).ReturnsAsync(author);

            var query = new GetPublicationByAuthorQuery { UserId = author.Id, PageNumber = 1, PageSize = 5 };

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

        private GetPublicationByAuthorQuery.GetPublicationByAuthorHandler CreateSut(ApplicationDbContext context)
            => new GetPublicationByAuthorQuery.GetPublicationByAuthorHandler(context, _userManagerMock.Object);
    }
}
