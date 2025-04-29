using Microsoft.AspNetCore.Identity;

namespace StageCraft.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;  // Store user's full name
    }
}
