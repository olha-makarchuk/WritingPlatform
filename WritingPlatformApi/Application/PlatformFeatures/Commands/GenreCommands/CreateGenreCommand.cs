using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Application.PlatformFeatures.Commands.GenreCommands
{
    public class CreateGenreCommand : IRequest<Genre>
    {
        public string Name { get; set; } = null!;
        public IFormFile FilePath { get; set; } = null!;
    }
    public class CreateMovieCommandHandler : IRequestHandler<CreateGenreCommand, Genre>
    {
        private readonly IApplicationDbContext _context;
        private IApiClientGoogleDrive _client;
        private readonly IBlobStorage _storage;

        public CreateMovieCommandHandler(IApplicationDbContext context, IApiClientGoogleDrive client, IBlobStorage storage)
        {
            _context = context;
            _client = client;
            _storage = storage;
        }

        public async Task<Genre> Handle(CreateGenreCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var genreFileName = $"{Guid.NewGuid()}_{timestamp}_{command.FilePath.FileName}";


            await using (var genreStream = command.FilePath.OpenReadStream())
            {
                await _storage.PutContextAsync(genreFileName, genreStream);
            }

            var genre = new Genre
            {
                Name = command.Name,
                FileKey = genreFileName
            };

            _context.Genre.Add(genre);
            await _context.SaveChangesAsync();

            return genre;
        }
    }
}
