using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Newtonsoft.Json.Linq;

namespace Application.PlatformFeatures.Commands.GenreCommands
{
    public class CreateGenreCommand : IRequest<Genre>
    {
        public string Name { get; set; } = null!;
        public string FilePath { get; set; } = null!;
    }
    public class CreateMovieCommandHandler : IRequestHandler<CreateGenreCommand, Genre>
    {
        private readonly IApplicationDbContext _context;
        private IApiClientGoogleDrive _client;

        public CreateMovieCommandHandler(IApplicationDbContext context, IApiClientGoogleDrive client)
        {
            _context = context;
            _client = client;
        }

        public async Task<Genre> Handle(CreateGenreCommand command, CancellationToken cancellationToken)
        {
            var fileId = _client.AddFile(command.FilePath, "GenreFolder");

            var genre = new Genre
            {
                Name = command.Name,
                FileKey = fileId
            };

            _context.Genre.Add(genre);
            await _context.SaveChangesAsync();

            return genre;
        }
    }
}
