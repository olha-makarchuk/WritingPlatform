using Application.Interfaces;
using Domain.Entities;
using Google.Apis.Drive.v3.Data;
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
        public string UserName { get; set; }
        public IFormFile FilePath { get; set; }
        public IFormFile TitlePath { get; set; }
        public string bookDescription { get; set; } = string.Empty;
    }

    public class CreatePublicationCommandHandler : IRequestHandler<CreatePublicationCommand, Publication>
    {
        private readonly IApplicationDbContext _context;
        private IApiClientGoogleDrive _client;
        private readonly UserManager<ApplicationUser> _userManager;

        // Define destination folders for storing files temporarily
        private readonly string _publicationFolder = "TempPublicationFolder";
        private readonly string _titleFolder = "TempTitleFolder";

        public CreatePublicationCommandHandler(IApplicationDbContext context, IApiClientGoogleDrive client, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _client = client;
            _userManager = userManager;
        }

        public async Task<Publication> Handle(CreatePublicationCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(command.UserName);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var existingPublications = await _context.Publication
                .Include(u => u.ApplicationUser)
                .Where(a => a.ApplicationUser.UserName == command.UserName).ToListAsync(cancellationToken);

            if (!existingPublications.Any())
            {
                user.IsAuthor = true;
                await _userManager.UpdateAsync(user);
            }

            // Save the files locally
            var publicationFilePath = await SaveFileAsync(command.FilePath, _publicationFolder);
            var titleFilePath = await SaveFileAsync(command.TitlePath, _titleFolder);

            // Upload files to Google Drive
            var IdFile = _client.AddFile(publicationFilePath, "PublicationFolder");
            var idTitle = _client.AddFile(titleFilePath, "TitleFolder");

            var countPages = _client.CountPages(publicationFilePath);

            var publication = new Publication
            {
                DatePublication = DateTime.Now,
                FileKey = IdFile,
                TitleKey = idTitle,
                GenreId = command.GenreId,
                PublicationName = command.PublicationName,
                Rating = 0,
                CountPages = countPages,
                ApplicationUserId = user.Id,
                bookDescription = command.bookDescription
            };

            _context.Publication.Add(publication);
            await _context.SaveChangesAsync(cancellationToken);

            // Optionally, delete the local files after upload
            System.IO.File.Delete(publicationFilePath);
            System.IO.File.Delete(titleFilePath);

            return publication;
        }

        private async Task<string> SaveFileAsync(IFormFile formFile, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            var fileName = Path.GetFileName(formFile.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(destinationFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            return filePath;
        }
    }

}
