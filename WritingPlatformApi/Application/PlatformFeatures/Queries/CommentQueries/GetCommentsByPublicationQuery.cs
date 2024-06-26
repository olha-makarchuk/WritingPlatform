using Application.Interfaces;
using Application.Services;
using Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Queries.CommentQueries
{
    public class GetCommentsByPublicationQuery : IRequest<List<CommentResponse>>
    {
        [Required(ErrorMessage = "IdPublication is required.")]
        public int IdPublication { get; set; }

        public class GetCommentByPublicationQuerieHandler : IRequestHandler<GetCommentsByPublicationQuery, List<CommentResponse>>
        {
            private readonly IApplicationDbContext _context;

            public GetCommentByPublicationQuerieHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<CommentResponse>> Handle(GetCommentsByPublicationQuery query, CancellationToken cancellationToken)
            {
                var a = await _context.Publication.ToListAsync(cancellationToken);
                var publicationExist = await _context.Publication
                    .FirstOrDefaultAsync(a => a.Id == query.IdPublication)
                    ?? throw new NotFoundException("Publication not found");

                return await _context.Comment
                    .Where(a => a.PublicationId == query.IdPublication)
                    .Select(a => new CommentResponse
                    {
                        CommentText = a.CommentText,
                        CommentId = a.Id,
                        UserId = a.ApplicationUserId
                    })
                    .ToListAsync(cancellationToken);
            }
        }
    }
}
