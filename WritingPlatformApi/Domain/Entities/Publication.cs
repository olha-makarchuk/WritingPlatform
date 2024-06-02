using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Publication")]
    public class Publication: BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string PublicationName { get; set; } = string.Empty;

        [Required]
        public int GenreId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        [Range(0, 100)]
        public int Rating { get; set; }

        [Required]
        public DateTime DatePublication {  get; set; }

        [Required]
        [StringLength(255)]
        public string FileKey { get; set; } = string.Empty;
        public string TitleKey { get; set; } = string.Empty;
        public int CountPages {  get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string bookDescription { get; set; } = string.Empty;

        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public virtual Genre Genre { get; set; } = null!;
    }
}
