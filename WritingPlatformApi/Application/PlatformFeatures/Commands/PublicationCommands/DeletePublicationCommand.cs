using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Commands.PublicationCommands
{
    public class DeletePublicationCommand : IRequest<Publication>
    {
        public int Id { get; set; }
    }

    public class DeleteMovieByIdCommandHandler : IRequestHandler<DeletePublicationCommand, Publication>
    {
        private readonly IApplicationDbContext _context;
        private IApiClientGoogleDrive _client;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteMovieByIdCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IApiClientGoogleDrive client)
        {
            _context = context;
            _userManager = userManager;
            _client = client;
        }

        public async Task<Publication> Handle(DeletePublicationCommand command, CancellationToken cancellationToken)
        {
            var publication = await _context.Publication.Where(a => a.Id == command.Id)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Publication not Found");

            _context.Publication.Remove(publication);
            await _context.SaveChangesAsync(cancellationToken);

            var publicationsUser = await _context.Publication
                .Where(p=>p.ApplicationUserId == publication.ApplicationUserId)
                .ToListAsync(cancellationToken);

            //_client.DeleteFile(, "PublicationFolder");

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
