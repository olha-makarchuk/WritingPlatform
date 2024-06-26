using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Commands.GenreCommands
{
    public class CreateGenreCommand : IRequest<Genre>
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(256, ErrorMessage = "Name cannot be longer than 256 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "FilePath is required.")]
        public IFormFile FilePath { get; set; } = null!;
    }
    public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, Genre>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBlobStorage _storage;

        public CreateGenreCommandHandler(IApplicationDbContext context,IBlobStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        public async Task<Genre> Handle(CreateGenreCommand command, CancellationToken cancellationToken)
        {
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
            await _context.SaveChangesAsync(cancellationToken);

            return genre;
        }
    }
}
