using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Commands.RewiewCommand
{
    public class CreateRewiewCommand : IRequest<Publication>
    {
        public int IdPublication { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
    }

    public class CreateRewiewCommandHandler : IRequestHandler<CreateRewiewCommand, Publication>
    {
        private readonly IApplicationDbContext _context;

        public CreateRewiewCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Publication> Handle(CreateRewiewCommand command, CancellationToken cancellationToken)
        {
            var publication = await _context.Publication.Where(u => u.Id == command.IdPublication).FirstOrDefaultAsync(cancellationToken);

            int newRating = 0;

            if (publication.CountOfRewiews == 0)
            {
                newRating = command.Rating;
            }
            else
            {
                newRating = (publication.Rating + command.Rating) / (publication.CountOfRewiews + 1);
            }

            publication.CountOfRewiews++;
            publication.Rating = newRating;

            _context.Publication.Update(publication);
            await _context.SaveChangesAsync();

            var rewiew = new UserRewiew()
            {
                PublicationId = command.IdPublication,
                Rewiew = command.Rating,
                ApplicationUserId = command.UserId
            };

            await _context.UserRewiew.AddAsync(rewiew);
            await _context.SaveChangesAsync(cancellationToken);

            return publication;
        }
    }
}
