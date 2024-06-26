using Application.PlatformFeatures.Commands.SortByItemCommands;
using AutoFixture;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.SortByItemCommandsTests
{
    public class CreateSortByItemCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly UserManagerDecorator _userManagerMock;
        private readonly CancellationTokenSource _cts = new();

        public CreateSortByItemCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _userManagerMock = new UserManagerDecorator();
        }

        [Fact]
        public async Task Handle_NewSortByItem_ShouldCreateSortByItem()
        {
            // Arrange
            var fixture = new Fixture();

            var command = new CreateSortByItemCommand
            {
                ItemName = "New Sort Item",
                FieldName = "new_sort_item"
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(command.ItemName, result.ItemName);
                Assert.Equal(command.FieldName, result.FieldName);

                var sortByItemFromDb = await context.SortByItem.FirstOrDefaultAsync(s => s.FieldName == command.FieldName);
                Assert.NotNull(sortByItemFromDb);
                Assert.Equal(command.ItemName, sortByItemFromDb.ItemName);
            });
        }

        [Fact]
        public async Task Handle_ExistingSortByItem_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var existingSortByItem = new SortByItem
            {
                ItemName = "Existing Item",
                FieldName = "existing_item"
            };

            _dbContext.AddAndSave(existingSortByItem);

            var command = new CreateSortByItemCommand
            {
                ItemName = "New Sort Item",
                FieldName = existingSortByItem.FieldName 
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, _cts.Token));
                Assert.Equal("SortByItem already exists", exception.Message);
            });
        }

        private static CreateSortByItemCommandHandler CreateSut(ApplicationDbContext context)
            => new CreateSortByItemCommandHandler(context);
    }
}
