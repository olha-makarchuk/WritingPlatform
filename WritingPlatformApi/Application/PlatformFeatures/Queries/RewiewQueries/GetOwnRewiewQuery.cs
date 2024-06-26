using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Queries.Review
{
    public class GetOwnReviewQuery : IRequest<UserRewiew>
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "PublicationId is required.")]
        public int PublicationId { get; set; }

        public class GetOwnReviewQueryHandler : IRequestHandler<GetOwnReviewQuery, UserRewiew>
        {
            private readonly IApplicationDbContext _context;

            public GetOwnReviewQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<UserRewiew> Handle(GetOwnReviewQuery query, CancellationToken cancellationToken)
            {
                var publication = await _context.Publication
                    .FirstOrDefaultAsync(u => u.Id == query.PublicationId, cancellationToken)
                    ?? throw new NotFoundException("Publication not found");

                var review = await _context.UserRewiew
                    .FirstOrDefaultAsync(u => u.ApplicationUserId == query.UserId && u.PublicationId == query.PublicationId, cancellationToken);

                return review;
            }
        }
    }
}
