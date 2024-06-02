using Application.Interfaces;
using Application.PlatformFeatures.Queries.PublicationQueries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.CommentQueries
{
    public class GetCommentsByPublicationQuery : IRequest<List<CommentTextModel>>
    {
        public int IdPublication { get; set; }

        public class GetCommentByPublicationQuerieHandler : IRequestHandler<GetCommentsByPublicationQuery, List<CommentTextModel>>
        {
            private readonly IApplicationDbContext _context;

            public GetCommentByPublicationQuerieHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<CommentTextModel>> Handle(GetCommentsByPublicationQuery query, CancellationToken cancellationToken)
            {
                var publicationExist = await _context.Publication.Where(a => a.Id == query.IdPublication)
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new Exception("Publication not found");

                return await _context.Comment
                    .Where(a => a.PublicationId == query.IdPublication)
                    .Select(a => new CommentTextModel
                    {
                        CommentText = a.CommentText,
                        CommentId = a.Id,
                        UserId = a.ApplicationUserId
                    })
                    .ToListAsync(cancellationToken);
            }
        }
    }

    public class CommentTextModel
    {
        public string CommentText { get; set; }
        public string UserId { get; set; }
        public int CommentId { get; set; }
    }
}
