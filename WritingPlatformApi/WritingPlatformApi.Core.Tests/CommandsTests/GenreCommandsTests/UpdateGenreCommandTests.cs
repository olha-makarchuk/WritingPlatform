using Application.Interfaces;
using Application.PlatformFeatures.Commands.GenreCommands;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Moq;
using Persistence.Context;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WritingPlatformApi.Core.Tests.CommandsTests.GenreCommands
{
    public class UpdateGenreCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();
        private readonly Mock<IBlobStorage> _storageMock;

        public UpdateGenreCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _storageMock = new Mock<IBlobStorage>();
        }

        [Fact]
        public async Task UpdateGenreCommand_IfGenreUpdates_ShouldReturnUpdatedGenre()
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

            Genre genre = new()
            {
                Id = 10,
                FileKey = "old_file_key",
                Name = "fileName"
            };

            _dbContext.AddAndSave(genre);

            UpdateGenreCommand updateGenreCommand = new()
            {
                Id = genre.Id,
                Name = "Updated Genre Name",
                FilePath = fileMock.Object
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object);

                // Act
                var result = await handler.Handle(updateGenreCommand, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(updateGenreCommand.Name, result.Name);
                Assert.NotNull(result.FileKey);
                Assert.NotEqual("old_file_key", result.FileKey);

                _storageMock.Verify(s => s.PutContextAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
                _storageMock.Verify(s => s.DeleteAsync("old_file_key"), Times.Once);

                var updatedGenre = await context.Genre.FindAsync(result.Id);
                Assert.NotNull(updatedGenre);
                Assert.Equal(result.Name, updatedGenre.Name);
                Assert.Equal(result.FileKey, updatedGenre.FileKey);
            });
        }
        
        [Fact]
        public async Task Handle_WhenGenreNotFound_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var updateGenreCommand = fixture.Build<UpdateGenreCommand>()
                .With(cg => cg.Id, 999)
                .With(cg => cg.Name, "Non-existing Genre")
                .With(cg => cg.FilePath, new Mock<IFormFile>().Object)
                .Create();

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(updateGenreCommand, _cts.Token));
            });
        }
        
        private static UpdateGenreCommandHandler CreateSut(ApplicationDbContext context, IBlobStorage storage)
            => new(context, storage);
    }
}
