using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Genre")]
    public class Genre: BaseEntity
    {
        [Required]
        public string Name { get; set; } = null!;
        public string FileKey { get; set; } = null!;
    }
}
