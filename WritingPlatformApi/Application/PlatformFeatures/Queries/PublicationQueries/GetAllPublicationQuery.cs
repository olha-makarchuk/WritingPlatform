using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetAllPublicationQuery : IRequest<IEnumerable<PublicationDto>>
    {
        public class GetAllMoviesQueryHandler : IRequestHandler<GetAllPublicationQuery, IEnumerable<PublicationDto>>
        {
            private readonly IApplicationDbContext _context;

            public GetAllMoviesQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<PublicationDto>> Handle(GetAllPublicationQuery request, CancellationToken cancellationToken)
            {
                var publicationList = await _context.Publication
                                .Include(p => p.Genre)
                                .Include(p => p.ApplicationUser)
                                .Select(p => new PublicationDto
                                {
                                    PublicationId = p.Id, 
                                    PublicationName = p.PublicationName,
                                    GenreName = p.Genre.Name,
                                    Author = new Author
                                    {
                                        FirstName = p.ApplicationUser.FirstName,
                                        LastName = p.ApplicationUser.LastName,
                                        UserName = p.ApplicationUser.UserName,
                                        PersonalInformation =p.ApplicationUser.PersonalInformation
                                    },
                                    Rating = p.Rating,
                                    TitleKey = p.TitleKey,
                                    FileKey = p.FileKey,
                                    DatePublication = p.DatePublication,
                                    bookDescription = p.bookDescription
                                })
                                .ToListAsync(cancellationToken);

                return publicationList;
            }
        }
    }

    public class PublicationDto
    {
        public int PublicationId { get; set; }
        public string PublicationName { get; set; }
        public string GenreName { get; set; }
        public Author Author { get; set; }
        public int Rating { get; set; }
        public string FileKey { get; set; }
        public string TitleKey { get; set; }
        public DateTime DatePublication { get; set; }
        public string bookDescription { get; set; }
    }

    public class Author
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalInformation {  get; set; }
    }
}
