using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByAuthorQuery : IRequest<List<PublicationDto>>
    {
        public string UserId { get; set; } =string.Empty;

        public class GetPublicationByAuthorHandler : IRequestHandler<GetPublicationByAuthorQuery, List<PublicationDto>>
        {
            private readonly IApplicationDbContext _context;
            private UserManager<ApplicationUser> _userManager;
            public GetPublicationByAuthorHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public async Task<List<PublicationDto>> Handle(GetPublicationByAuthorQuery query, CancellationToken cancellationToken)
            {
                var authorExist = await _userManager.FindByIdAsync(query.UserId);
                if (authorExist == null || !authorExist.IsAuthor)
                {
                    throw new Exception("Author not found");
                }

               var publicationList = await _context.Publication
               .Where(a => a.ApplicationUserId == query.UserId)
               .Include(p => p.Genre)
               .Include(p => p.ApplicationUser)
               .Select(p => new PublicationDto
               {
                   PublicationId = p.Id,
                   PublicationName = p.PublicationName,
                   GenreName = p.Genre.Name,
                   Author = new Author
                   {
                       UserName = p.ApplicationUser.UserName,
                       FirstName = p.ApplicationUser.FirstName,
                       LastName = p.ApplicationUser.LastName,
                       PersonalInformation = p.ApplicationUser.PersonalInformation
                   },
                   Rating = p.Rating,
                   TitleKey = p.TitleKey,
                   FileKey = p.FileKey,
                   DatePublication = p.DatePublication,
                   bookDescription = p.bookDescription
               })
               .ToListAsync(cancellationToken)
                ?? throw new Exception("Publication not found");

                return publicationList;
            }
        }
    }
}
