using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class UserRewiew: BaseEntity
    {
        [Required]
        [StringLength(500)]
        public int Rewiew { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public int PublicationId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; } = null!;

        public virtual Publication Publication { get; set; } = null!;
    }
}
