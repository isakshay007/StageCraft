using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StageCraft.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment text is required.")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Use UTC for consistency

        // Production Foreign Key
        [Required]
        public int ProductionId { get; set; }

        [ForeignKey("ProductionId")]
        public Production Production { get; set; } = null!;  // Avoid nullable warnings

        // User Foreign Key
        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;  // Avoid nullable warnings

        //   Convenience property to easily get Username for comments
        [NotMapped]
        public string Username => User?.UserName ?? "Unknown User"; 
    }
}
