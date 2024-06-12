using Application.PlatformFeatures.Queries.UserAccountQueries;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.QueriesTests.UserAccountQueriesTests
{
    public class GetUserAccountByLoginQueryTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;

        private readonly UserManagerDecorator _userManagerMock;

        protected readonly CancellationTokenSource _cts = new();

        public GetUserAccountByLoginQueryTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = new UserManagerDecorator();
        }

        [Fact]
        public async Task GetUserAccountByLoginQuery_WithValidUserId_ShouldReturnUserAccount()
        {
            // Arrange
            var fixture = new Fixture();
            var user = fixture.Build<ApplicationUser>().Create();
            _dbContext.AddAndSave(user);

            _userManagerMock.SetupFindByIdAsync(user);

            var query = new GetUserAccountByLoginQuery { UserId = user.Id };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(query, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(user.UserName, result.UserName);
                Assert.Equal(user.Email, result.Email);
                Assert.Equal(user.PersonalInformation, result.PersonalInformation);
                Assert.Equal(user.FirstName, result.FirstName);
                Assert.Equal(user.LastName, result.LastName);
            });
        }

        [Fact]
        public async Task GetUserAccountByLoginQuery_WithInvalidUserId_ShouldThrowException()
        {
            // Arrange
            var query = new GetUserAccountByLoginQuery { UserId = "invalidUserId" };

            _userManagerMock.SetupFindByIdAsync((ApplicationUser)null!);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        [Fact]
        public async Task GetUserAccountByLoginQuery_WithNoUser_ShouldThrowException()
        {
            // Arrange
            var query = new GetUserAccountByLoginQuery { UserId = "nonExistentUserId" };

            _userManagerMock.SetupFindByIdAsync((ApplicationUser)null!);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act and Assert
                await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(query, _cts.Token));
            });
        }

        private GetUserAccountByLoginQueryHandler CreateSut(ApplicationDbContext context)
            => new GetUserAccountByLoginQueryHandler(context, _userManagerMock.UserManagerMock.Object);
    }
}
