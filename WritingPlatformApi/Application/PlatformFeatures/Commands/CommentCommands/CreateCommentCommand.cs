using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Commands.CommentCommands
{
    public class CreateCommentCommand : IRequest<Comment>
    {
        [Required(ErrorMessage = "CommentText is required.")]
        [MaxLength(500, ErrorMessage = "CommentText cannot be longer than 256 characters.")]
        public string CommentText { get; set; } = string.Empty;

        [Required(ErrorMessage = "ApplicationUserId is required.")]
        public string ApplicationUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "PublicationId is required.")]
        public int PublicationId { get; set; }
    }

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, Comment>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateCommentCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Comment> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.ApplicationUserId)
                ?? throw new NotFoundException($"User with ID {command.ApplicationUserId} not found.");

            var publication = _context.Publication.Where(u => u.Id == command.PublicationId)
                ?? throw new NotFoundException($"Publication with ID {command.PublicationId} not found.");

            var comment = new Comment
            {
                ApplicationUserId = command.ApplicationUserId,
                CommentText = command.CommentText,
                PublicationId = command.PublicationId
            };

            _context.Comment.Add(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return comment;
        }
    }
}
