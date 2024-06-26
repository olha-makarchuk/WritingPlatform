using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Commands.RewiewCommand
{
    public class CreateRewiewCommand : IRequest<Publication>
    {
        [Required(ErrorMessage = "IdPublication is required.")]
        public int IdPublication { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        [Range(0, 100, ErrorMessage = "Rating must be between 0 and 100.")]
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
            var existingReview = await _context.UserRewiew
                .FirstOrDefaultAsync(r => r.ApplicationUserId == command.UserId && r.PublicationId == command.IdPublication);

            if (existingReview != null)
            {
                throw new ArgumentException("User has already reviewed this publication.");
            }

            var publication = await _context.Publication.FirstOrDefaultAsync(u => u.Id == command.IdPublication, cancellationToken);
            if (publication == null) throw new NotFoundException("Publication not found");

            int newRating = (publication.CountOfRewiews == 0) ? command.Rating :
                            (publication.Rating + command.Rating) / (publication.CountOfRewiews + 1);

            publication.CountOfRewiews++;
            publication.Rating = newRating;

            _context.Publication.Update(publication);
            await _context.SaveChangesAsync(cancellationToken);

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
