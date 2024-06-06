using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Commands.PublicationCommands
{
    public class DeletePublicationByIdCommand : IRequest<Publication>
    {
        public int Id { get; set; }
    }

    public class DeletePublicationCommandHandler : IRequestHandler<DeletePublicationByIdCommand, Publication>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBlobStorage _storage;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeletePublicationCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IBlobStorage storage)
        {
            _context = context;
            _userManager = userManager;
            _storage = storage;
        }

        public async Task<Publication> Handle(DeletePublicationByIdCommand command, CancellationToken cancellationToken)
        {
            var publication = await _context.Publication.Where(a => a.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Publication not Found");

            await _storage.DeleteAsync(publication.FileKey);
            await _storage.DeleteAsync(publication.TitleKey);

            var comments = await _context.Comment.Where(a => a.PublicationId == publication.Id).ToListAsync(cancellationToken);
            _context.Comment.RemoveRange(comments);
            await _context.SaveChangesAsync(cancellationToken);

            _context.Publication.Remove(publication);
            await _context.SaveChangesAsync(cancellationToken);

            var publicationsUser = await _context.Publication
                .Where(p=>p.ApplicationUserId == publication.ApplicationUserId)
                .ToListAsync(cancellationToken);

            if (publicationsUser == null )
            {
                var userExists = await _userManager.FindByIdAsync(publication.ApplicationUserId);
                userExists.IsAuthor = false;
                await _userManager.UpdateAsync(userExists);
            }

            return publication;
        }
    }
}
