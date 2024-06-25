using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using iTextSharp.text.pdf;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Commands.PublicationCommands
{
    public class CreatePublicationCommand : IRequest<Publication>
    {
        public string PublicationName { get; set; } = string.Empty;
        public int GenreId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public IFormFile FilePath { get; set; }
        public IFormFile TitlePath { get; set; }
        public string BookDescription { get; set; } = string.Empty;
    }

    public class CreatePublicationCommandHandler : IRequestHandler<CreatePublicationCommand, Publication>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBlobStorage _storage;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPdfReaderService _pdfReaderService;

        public CreatePublicationCommandHandler(IApplicationDbContext context, IBlobStorage storage, UserManager<ApplicationUser> userManager, IPdfReaderService pdfReaderService)
        {
            _context = context;
            _storage = storage;
            _userManager = userManager;
            _pdfReaderService = pdfReaderService;
        }

        public async Task<Publication> Handle(CreatePublicationCommand command, CancellationToken cancellationToken)
        {
            ValidatePdfFile(command.FilePath);
            ValidateJpegFile(command.TitlePath);

            var user = await _userManager.FindByIdAsync(command.UserId)
                ?? throw new NotFoundException("User not found");

            var genre = await _context.Genre.FirstOrDefaultAsync(u => u.Id == command.GenreId)
                ?? throw new NotFoundException("Genre not found");

            var existingPublications = await _context.Publication
                .Include(u => u.ApplicationUser)
                .Where(a => a.ApplicationUser.Id == command.UserId)
                .ToListAsync(cancellationToken);

            if (!existingPublications.Any())
            {
                user.IsAuthor = true;
                await _userManager.UpdateAsync(user);
            }

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var publicationFileName = $"{Guid.NewGuid()}_{timestamp}_{command.FilePath.FileName}";
            var titleFileName = $"{Guid.NewGuid()}_{timestamp}_{command.TitlePath.FileName}";

            int pageCount;
            await using (var publicationStream = command.FilePath.OpenReadStream())
            {
                await _storage.PutContextAsync(publicationFileName, publicationStream);
                publicationStream.Position = 0;
                pageCount = _pdfReaderService.GetPageCount(publicationStream);
            }

            await using (var titleStream = command.TitlePath.OpenReadStream())
            {
                await _storage.PutContextAsync(titleFileName, titleStream);
            }

            var publication = new Publication
            {
                DatePublication = DateTime.Now,
                FileKey = publicationFileName,
                TitleKey = titleFileName,
                GenreId = command.GenreId,
                PublicationName = command.PublicationName,
                Rating = 0,
                CountOfPages = pageCount,
                ApplicationUserId = user.Id,
                bookDescription = command.BookDescription
            };

            await _context.Publication.AddAsync(publication, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return publication;
        }

        private static void ValidatePdfFile(IFormFile file)
        {
            if (file == null || file.ContentType != "application/pdf")
            {
                throw new InvalidOperationException("Only PDF files are allowed for FilePath.");
            }
        }

        private static void ValidateJpegFile(IFormFile file)
        {
            if (file == null || (file.ContentType != "image/jpeg" && file.ContentType != "image/jpg"))
            {
                throw new InvalidOperationException("Only JPEG files are allowed for TitlePath.");
            }
        }
    }
}
