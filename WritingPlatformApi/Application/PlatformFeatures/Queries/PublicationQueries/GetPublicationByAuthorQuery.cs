using Application.Interfaces;
using Application.Services;
using Contracts.Responses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByAuthorQuery : IRequest<List<PublicationResponse>>
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; } =string.Empty;

        [Required(ErrorMessage = "PageNumber is required.")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "PageSize is required.")]
        public int PageSize { get; set; }

        public class GetPublicationByAuthorHandler : IRequestHandler<GetPublicationByAuthorQuery, List<PublicationResponse>>
        {
            private readonly IApplicationDbContext _context;
            private UserManager<ApplicationUser> _userManager;
            public GetPublicationByAuthorHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public async Task<List<PublicationResponse>> Handle(GetPublicationByAuthorQuery query, CancellationToken cancellationToken)
            {
                var authorExist = await _userManager.FindByIdAsync(query.UserId);
                if (authorExist == null)
                {
                    throw new NotFoundException("Author not found");
                }

                var totalPublications = await _context.Publication.Where(u => u.ApplicationUserId == query.UserId).CountAsync();

                var publicationList = await _context.Publication
               .Where(a => a.ApplicationUserId == query.UserId)
               .Include(p => p.Genre)
               .Include(p => p.ApplicationUser)
               .Skip((query.PageNumber - 1) * query.PageSize)
               .Take(query.PageSize)
               .OrderBy(g => g.PublicationName)
               .Select(p => new PublicationResponse
               {
                   PublicationId = p.Id,
                   PublicationName = p.PublicationName,
                   GenreName = p.Genre.Name,
                   Author = new AuthorResponse
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
                   bookDescription = p.bookDescription,
                   PaginatorCount = (int)Math.Ceiling((double)totalPublications / query.PageSize)
               })
               .ToListAsync(cancellationToken)
                ?? throw new NotFoundException("Publication not found");

                return publicationList;
            }
        }
    }
}
