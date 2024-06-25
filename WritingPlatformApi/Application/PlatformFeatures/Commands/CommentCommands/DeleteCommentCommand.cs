using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Commands.CommentCommands
{
    public class DeleteCommentCommand : IRequest<Comment>
    {
        public int CommentId { get; set; }
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, Comment>
    {
        private readonly IApplicationDbContext _context;

        public DeleteCommentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {
            var comment = await _context.Comment.FirstOrDefaultAsync(u => u.Id == command.CommentId, cancellationToken)
                ?? throw new NotFoundException($"Comment with ID {command.CommentId} not found.");

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return comment;
        }
    }
}
