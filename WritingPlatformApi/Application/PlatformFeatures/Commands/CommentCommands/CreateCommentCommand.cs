using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.PlatformFeatures.Commands.CommentCommands
{
    public class CreateCommentCommand : IRequest<Comment>
    {
        public string CommentText { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;
        public int PublicationId { get; set; }
    }

    public class CreateMovieCommandHandler : IRequestHandler<CreateCommentCommand, Comment>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateMovieCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Comment> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = new Comment
            {
                ApplicationUserId = request.ApplicationUserId,
                CommentText = request.CommentText,
                PublicationId = request.PublicationId
            };

            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();

            return comment;
        }
    }
}
