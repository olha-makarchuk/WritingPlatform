using Application.Interfaces;
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
        private IApiClientGoogleDrive _client;

        public DeleteCommentCommandHandler(IApplicationDbContext context, IApiClientGoogleDrive client)
        {
            _context = context;
            _client = client;
        }

        public async Task<Comment> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {
            var comment = await _context.Comment.Where(u => u.Id == command.CommentId).FirstOrDefaultAsync();
            
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            return comment;
        }
    }
}
