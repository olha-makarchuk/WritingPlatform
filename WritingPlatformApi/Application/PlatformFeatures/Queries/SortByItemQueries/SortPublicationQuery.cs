using Application.Interfaces;
using Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Queries.SortByItemQueries
{
    public class SortPublicationQuery : IRequest<List<SortPublicationResponse>>
    {
        [Required(ErrorMessage = "GenreIds is required.")]
        public List<int> GenreIds { get; set; }

        [Required(ErrorMessage = "StartPage is required.")]
        public int StartPage { get; set; }

        [Required(ErrorMessage = "EndPage is required.")]
        public int EndPage { get; set; }

        [Required(ErrorMessage = "YearPublication is required.")]
        public int YearPublication { get; set; }

        [Required(ErrorMessage = "SortBy is required.")]
        public int SortBy { get; set; }

        [Required(ErrorMessage = "SortDirection is required.")]
        public string SortDirection { get; set; }

        public class SortPublicationQueryHandler : IRequestHandler<SortPublicationQuery, List<SortPublicationResponse>>
        {
            private readonly IApplicationDbContext _context;

            public SortPublicationQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<SortPublicationResponse>> Handle(SortPublicationQuery query, CancellationToken cancellationToken)
            {
                var publicationsQuery = await _context.Publication
                    .Include(p => p.Genre)
                    .Include(p => p.ApplicationUser)
                    .Select(p => new SortPublicationResponse
                    {
                        PublicationId = p.Id,
                        PublicationName = p.PublicationName,
                        GenreName = p.Genre.Name,
                        Author = new AuthorResponse
                        {
                            FirstName = p.ApplicationUser.FirstName,
                            LastName = p.ApplicationUser.LastName,
                            UserName = p.ApplicationUser.UserName,
                            PersonalInformation = p.ApplicationUser.PersonalInformation
                        },
                        Rating = p.Rating,
                        TitleKey = p.TitleKey,
                        FileKey = p.FileKey,
                        DatePublication = p.DatePublication,
                        bookDescription = p.bookDescription,
                        GenreId = p.GenreId,
                        CountPages = p.CountOfPages
                    })
                    .ToListAsync(cancellationToken);

                if (query.GenreIds.Count != 0)
                {
                    publicationsQuery = publicationsQuery.Where(u => query.GenreIds.Contains(u.GenreId)).ToList();
                }

                if (query.YearPublication != 0)
                {
                    publicationsQuery = publicationsQuery.Where(u => u.DatePublication.Year == query.YearPublication).ToList();
                }

                if (query.StartPage != 0 && query.EndPage != 0)
                {
                    publicationsQuery = publicationsQuery.Where(u => u.CountPages <= query.EndPage && u.CountPages >= query.StartPage).ToList();
                }

                List<PublicationWithCommentsCount> publicationsWithCommentsCountQuery = publicationsQuery
                    .GroupJoin(
                        _context.Comment,
                        publication => publication.PublicationId,
                        comment => comment.PublicationId,
                        (publication, comments) => new PublicationWithCommentsCount
                        {
                            Publication = publication,
                            CommentsCount = comments.Count()
                        }).ToList();

                var sortByItem = await _context.SortByItem.FirstOrDefaultAsync(item => item.Id == query.SortBy, cancellationToken);

                if (sortByItem == null)
                {
                    throw new ArgumentException("Invalid sort by item");
                }

                IOrderedEnumerable<PublicationWithCommentsCount> orderedPublicationsQuery;
                switch (sortByItem.FieldName)
                {
                    case "Rating":
                        orderedPublicationsQuery = query.SortDirection.ToLower() == "asc" ?
                            publicationsWithCommentsCountQuery.OrderBy(u => u.Publication.Rating) :
                            publicationsWithCommentsCountQuery.OrderByDescending(u => u.Publication.Rating);
                        break;
                    case "DateAdding":
                        orderedPublicationsQuery = query.SortDirection.ToLower() == "asc" ?
                            publicationsWithCommentsCountQuery.OrderBy(u => u.Publication.DatePublication) :
                            publicationsWithCommentsCountQuery.OrderByDescending(u => u.Publication.DatePublication);
                        break;
                    case "NumberReviews":
                        orderedPublicationsQuery = query.SortDirection.ToLower() == "asc" ?
                            publicationsWithCommentsCountQuery.OrderBy(u => u.CommentsCount) :
                            publicationsWithCommentsCountQuery.OrderByDescending(u => u.CommentsCount);
                        break;
                    default:
                        throw new ArgumentException("Invalid sort by field");
                }


                var sortedPublications = orderedPublicationsQuery.Select(a => a.Publication).ToList();
                return sortedPublications;
            }
        }
    }
    public class PublicationWithCommentsCount
    {
        public SortPublicationResponse Publication { get; set; }
        public int CommentsCount { get; set; }
    }
}
