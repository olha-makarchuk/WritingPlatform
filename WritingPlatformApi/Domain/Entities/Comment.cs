using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Comment")]
    public class Comment: BaseEntity
    {
        [Required]
        [StringLength(500)]
        public string CommentText { get; set; } = string.Empty;
        
        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public int PublicationId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public virtual Publication Publication { get; set; } = null!;
    }
}
