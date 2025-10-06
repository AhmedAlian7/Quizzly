using Microsoft.AspNetCore.Identity;

namespace Quizzly.DataAccess.Entities
{
    public class ApplicationUser : IdentityUser
    {
        // BaseEntity properties
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;


        public string FirstName { get; set; }
        public string LastName { get; set; }

    }
}
