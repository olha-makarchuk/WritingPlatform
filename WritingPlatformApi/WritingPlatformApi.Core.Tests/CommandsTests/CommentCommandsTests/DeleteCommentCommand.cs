using Application.PlatformFeatures.Commands.CommentCommands;
using AutoFixture;
using Domain.Entities;
using Persistence.Context;

namespace WritingPlatformApi.Core.Tests.CommandsTests.CommentCommands
{
    public class DeleteCommentCommand
    {
        private readonly DbContextDecorator<ApplicationDbContext> _dbContext;
        protected readonly CancellationTokenSource _cts = new();

        public DeleteCommentCommand()
        {
            var options = Utilities.CreateInMemoryDbOptions<ApplicationDbContext>();

            _dbContext = new DbContextDecorator<ApplicationDbContext>(options);
        }

        [Fact]
        public async Task DeleteCommentCommand_IfCommentDeletes_ShouldReturnDeletedComment()
        {
            // Arrange
            var fixture = new Fixture();

            var comment = fixture.Build<Comment>()
                .With(c => c.Id, 1)
                .Create();

            _dbContext.AddAndSave(comment);

            var deleteCommentCommand = new Application.PlatformFeatures.Commands.CommentCommands.DeleteCommentCommand
            {
                CommentId = comment.Id
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act
                var result = await handler.Handle(deleteCommentCommand, _cts.Token);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(comment.Id, result.Id);

                var deletedComment = await context.Comment.FindAsync(comment.Id);
                Assert.Null(deletedComment); 
            });
        }

        [Fact]
        public async Task Handle_WhenCommentDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var fixture = new Fixture();

            var deleteCommentCommand = new Application.PlatformFeatures.Commands.CommentCommands.DeleteCommentCommand
            {
                CommentId = 999 
            };

            _dbContext.Assert(async context =>
            {
                var handler = CreateSut(context);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => handler.Handle(deleteCommentCommand, _cts.Token));
                Assert.Equal($"Comment with ID {deleteCommentCommand.CommentId} not found.", exception.Message);
            });
        }

        private static DeleteCommentCommandHandler CreateSut(ApplicationDbContext context)=> new(context);
    }
}
