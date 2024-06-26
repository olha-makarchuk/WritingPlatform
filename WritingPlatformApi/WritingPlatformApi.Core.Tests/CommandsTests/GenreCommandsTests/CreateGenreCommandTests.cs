using Application.Interfaces;
using Application.PlatformFeatures.Commands.GenreCommands;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Moq;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.GenreCommands
{
    public class CreateGenreCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();
        private readonly Mock<IBlobStorage> _storageMock;

        public CreateGenreCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _storageMock = new Mock<IBlobStorage>();
        }

        [Fact]
        public async Task CreateGenreCommand_IfGenreCreates_ShouldReturnCreatedGenre()
        {
            // Arrange
            var fixture = new Fixture();

            var fileMock = new Mock<IFormFile>();
            var content = "Fake file content";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            Application.PlatformFeatures.Commands.GenreCommands.CreateGenreCommand createGenreCommand = new()
            {
                FilePath = fileMock.Object,
                Name = "Action"
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object);

                // Act
                var result = await handler.Handle(createGenreCommand, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(createGenreCommand.Name, result.Name);
                Assert.NotNull(result.FileKey);

                _storageMock.Verify(s => s.PutContextAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);

                var createdGenre = await context.Genre.FindAsync(result.Id);
                Assert.NotNull(createdGenre);
                Assert.Equal(result.Name, createdGenre.Name);
                Assert.Equal(result.FileKey, createdGenre.FileKey);
            });
        }

        [Fact]
        public async Task Handle_WhenNameOrFilePathIsNull_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            CreateGenreCommand createGenreCommand = new()
            {
                FilePath = (IFormFile)null!,
                Name = (string)null!
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(createGenreCommand, _cts.Token));
            });
        }

        private static CreateGenreCommandHandler CreateSut(ApplicationDbContext context, IBlobStorage storage)
            => new(context, storage);
    }
}
