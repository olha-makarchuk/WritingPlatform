﻿using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByIdQuery : IRequest<PublicationTextDto>
    {
        public int IdPublication { get; set; }

        public class GetPublicationByIdQueryHandler : IRequestHandler<GetPublicationByIdQuery, PublicationTextDto>
        {
            private readonly IApplicationDbContext _context;
            private IApiClientGoogleDrive _client;

            public GetPublicationByIdQueryHandler(IApplicationDbContext context, IApiClientGoogleDrive client)
            {
                _context = context;
                _client = client;
            }

            public async Task<PublicationTextDto> Handle(GetPublicationByIdQuery query, CancellationToken cancellationToken)
            {
                //string text = _client.GetTextFromDocxFile("1eILVfqB7x22UMoaHfMQASUiZIib9oTuT", 1);

                var publication = await _context.Publication
               .Where(a => a.Id == query.IdPublication)
               .Include(p => p.Genre)
               .Include(p => p.ApplicationUser)
               .Select(p => new PublicationTextDto
               {
                   PublicationId = p.Id,
                   PublicationName = p.PublicationName,
                   GenreName = p.Genre.Name,
                   Rating = p.Rating,
                   TitleKey = p.TitleKey,
                   Author = new Author
                   {
                       UserName = p.ApplicationUser.UserName,
                       FirstName = p.ApplicationUser.FirstName,
                       LastName = p.ApplicationUser.LastName,
                       PersonalInformation = p.ApplicationUser.PersonalInformation
                   },
                   bookDescription = p.bookDescription,
                   //FileKey = p.FileKey,
                   //Text = text,
                   DatePublication = p.DatePublication
               })
               .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Publication not found");

                return publication;
            }
        }
    }

    public class PublicationTextDto
    {
        public int PublicationId { get; set; }
        public string PublicationName { get; set; }
      //public string Text { get; set; }
        public string GenreName { get; set; }
        public int Rating { get; set; }
      //public string FileKey { get; set; }
        public string TitleKey { get; set; }
        public DateTime DatePublication { get; set; }
        public Author Author {  get; set; }
        public string bookDescription { get; set; }
    }
}
