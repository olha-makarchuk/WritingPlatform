using Application.Interfaces;
using Application.PlatformFeatures.Commands.PublicationCommands;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using Persistence.Context;
using Xunit;

namespace WritingPlatformApi.Core.Tests.CommandsTests.PublicationCommands
{
    public class CreatePublicationCommandTests
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        private readonly Mock<IBlobStorage> _storageMock;
        private readonly UserManagerDecorator _userManagerDecorator;
        private readonly Mock<IPdfReaderService> _pdfReaderServiceMock; 
        protected readonly CancellationTokenSource _cts = new();
        public CreatePublicationCommandTests()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();
            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
            _storageMock = new Mock<IBlobStorage>();
            _userManagerDecorator = new UserManagerDecorator();
            _pdfReaderServiceMock = new Mock<IPdfReaderService>();
        }

        [Fact]
        public async Task CreatePublicationCommand_IfPublicationCreates_ShouldReturnCreatedPublication()
        {
            // Arrange
            var fixture = new Fixture();

            var fileMock = new Mock<IFormFile>();
            var content = "Fake PDF content";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.ContentType).Returns("application/pdf");
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var titleMock = new Mock<IFormFile>();
            var titleContent = "Fake JPEG content";
            var titleFileName = "title.jpg";
            var titleMs = new MemoryStream();
            var titleWriter = new StreamWriter(titleMs);
            titleWriter.Write(titleContent);
            titleWriter.Flush();
            titleMs.Position = 0;
            titleMock.Setup(_ => _.OpenReadStream()).Returns(titleMs);
            titleMock.Setup(_ => _.FileName).Returns(titleFileName);
            titleMock.Setup(_ => _.ContentType).Returns("image/jpeg");
            titleMock.Setup(_ => _.Length).Returns(titleMs.Length);

            var command = new CreatePublicationCommand
            {
                PublicationName = "Test Publication",
                GenreId = 1,
                UserId = "test-user-id",
                FilePath = fileMock.Object,
                TitlePath = titleMock.Object,
                BookDescription = "Test Description"
            };

            var user = new ApplicationUser { Id = "test-user-id", UserName = "testuser", IsAuthor = false };
            var genre = new Genre { Id = 1, Name = "Test Genre", FileKey="GenreFileKey"};

            _userManagerDecorator.SetupFindByIdAsync(user);
            _pdfReaderServiceMock.Setup(m => m.GetPageCount(It.IsAny<Stream>())).Returns(10); 

            _dbContext.AddAndSave(genre);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object, _userManagerDecorator.UserManagerMock.Object, _pdfReaderServiceMock.Object);

                // Act
                var result = await handler.Handle(command, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(command.PublicationName, result.PublicationName);
                Assert.Equal(command.GenreId, result.GenreId);
                Assert.Equal(command.BookDescription, result.bookDescription);
                Assert.Equal(user.Id, result.ApplicationUserId);

                _storageMock.Verify(s => s.PutContextAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Exactly(2));

                var createdPublication = await context.Publication.FindAsync(result.Id);
                Assert.NotNull(createdPublication);
                Assert.Equal(result.PublicationName, createdPublication.PublicationName);
            });
        }

        [Fact]
        public async Task Handle_WhenFilePathIsNotPdf_ShouldThrowException()
        {
            // Arrange
            var command = new CreatePublicationCommand
            {
                FilePath = null!,
                TitlePath = new Mock<IFormFile>().Object,
                GenreId = 1,
                UserId = "test-user-id",
                PublicationName = "Invalid PDF",
                BookDescription = "Description"
            };
            _pdfReaderServiceMock.Setup(m => m.GetPageCount(It.IsAny<Stream>())).Returns(10);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object, _userManagerDecorator.UserManagerMock.Object, _pdfReaderServiceMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, _cts.Token));
                Assert.Equal("Only PDF files are allowed for FilePath.", exception.Message);
            });
        }

        [Fact]
        public async Task Handle_WhenTitlePathIsNotJpeg_ShouldThrowException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.ContentType).Returns("application/pdf");

            var command = new CreatePublicationCommand
            {
                FilePath = fileMock.Object,
                TitlePath = null!, // Invalid file
                GenreId = 1,
                UserId = "test-user-id",
                PublicationName = "Invalid JPEG",
                BookDescription = "Description"
            };
            _pdfReaderServiceMock.Setup(m => m.GetPageCount(It.IsAny<Stream>())).Returns(10);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object, _userManagerDecorator.UserManagerMock.Object, _pdfReaderServiceMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, _cts.Token));
                Assert.Equal("Only JPEG files are allowed for TitlePath.", exception.Message);
            });
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.ContentType).Returns("application/pdf");

            var titleMock = new Mock<IFormFile>();
            titleMock.Setup(_ => _.ContentType).Returns("image/jpeg");

            var command = new CreatePublicationCommand
            {
                FilePath = fileMock.Object,
                TitlePath = titleMock.Object,
                GenreId = 1,
                UserId = "non-existent-user-id",
                PublicationName = "Non-existent User",
                BookDescription = "Description"
            };
            _pdfReaderServiceMock.Setup(m => m.GetPageCount(It.IsAny<Stream>())).Returns(10);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object, _userManagerDecorator.UserManagerMock.Object, _pdfReaderServiceMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, _cts.Token));
            });
        }

        [Fact]
        public async Task Handle_WhenGenreNotFound_ShouldThrowException()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.ContentType).Returns("application/pdf");

            var titleMock = new Mock<IFormFile>();
            titleMock.Setup(_ => _.ContentType).Returns("image/jpeg");

            var command = new CreatePublicationCommand
            {
                FilePath = fileMock.Object,
                TitlePath = titleMock.Object,
                GenreId = 999,
                UserId = "test-user-id",
                PublicationName = "Non-existent Genre",
                BookDescription = "Description"
            };

            var user = new ApplicationUser { Id = "test-user-id", UserName = "testuser", IsAuthor = false };
            _userManagerDecorator.SetupFindByIdAsync(user);
            _pdfReaderServiceMock.Setup(m => m.GetPageCount(It.IsAny<Stream>())).Returns(10);

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context, _storageMock.Object, _userManagerDecorator.UserManagerMock.Object, _pdfReaderServiceMock.Object);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, _cts.Token));
            });
        }

        private static CreatePublicationCommandHandler CreateSut(
            ApplicationDbContext context, 
            IBlobStorage storage, 
            UserManager<ApplicationUser> userManager, 
            IPdfReaderService pdfReader)
            => new(context, storage, userManager, pdfReader);
    }
}
