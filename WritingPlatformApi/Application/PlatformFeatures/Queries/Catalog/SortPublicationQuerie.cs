using Application.Interfaces;
using Application.PlatformFeatures.Queries.CommentQueries;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.Catalog
{
    public class SortPublicationQuery : IRequest<List<CommentTextModel>>
    {
        public List<int> GenreIds { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int YearPublication { get; set; }
        public SortByItem SortBy { get; set; }

        public class SortPublicationQueryHandler : IRequestHandler<SortPublicationQuery, List<CommentTextModel>>
        {
            private readonly IApplicationDbContext _context;

            public SortPublicationQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<CommentTextModel>> Handle(SortPublicationQuery query, CancellationToken cancellationToken)
            {
                IQueryable<Publication> publicationsQuery = _context.Publication;

                if (query.GenreIds.Count != 0)
                {
                    publicationsQuery = publicationsQuery.Where(u => query.GenreIds.Contains(u.GenreId));
                }

                if (query.YearPublication != 0)
                {
                    publicationsQuery = publicationsQuery.Where(u => u.DatePublication.Year == query.YearPublication);
                }

                if (query.StartPage != 0 && query.EndPage != 0)
                {
                    publicationsQuery = publicationsQuery.Where(u => u.CountPages<= query.EndPage && u.CountPages >= query.StartPage);
                }

                IQueryable<PublicationWithCommentsCount> publicationsWithCommentsCountQuery = publicationsQuery
                    .GroupJoin(
                        _context.Comment,
                        publication => publication.Id,
                        comment => comment.PublicationId,
                        (publication, comments) => new PublicationWithCommentsCount
                        {
                            Publication = publication,
                            CommentsCount = comments.Count()
                        });

                switch (query.SortBy)
                {
                    case SortByItem.Rating:
                        publicationsWithCommentsCountQuery = publicationsWithCommentsCountQuery.OrderByDescending(u => u.Publication.Rating);
                        break;
                    case SortByItem.DateAdding:
                        publicationsWithCommentsCountQuery = publicationsWithCommentsCountQuery.OrderByDescending(u => u.Publication.DatePublication);
                        break;
                    case SortByItem.NumberReviews:
                        publicationsWithCommentsCountQuery = publicationsWithCommentsCountQuery.OrderByDescending(u => u.CommentsCount);
                        break;
                    default:
                        publicationsWithCommentsCountQuery = publicationsWithCommentsCountQuery.OrderByDescending(u => u.Publication.Rating);
                        break;
                }

                List<CommentTextModel> commentTextModels = await publicationsWithCommentsCountQuery
                    .Select(p => new CommentTextModel
                    {
                    })
                    .ToListAsync(cancellationToken);

                return commentTextModels;
            }

            public class PublicationWithCommentsCount
            {
                public Publication Publication { get; set; }
                public int CommentsCount { get; set; }
            }


        }
    }

    public enum SortByItem
    {
        Rating,
        DateAdding,
        NumberReviews
    }
}
