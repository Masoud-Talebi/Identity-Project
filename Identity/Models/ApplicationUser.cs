using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        [MaxLength(150)]
        public required string FirstName { get; set; }
        [Required]
        [MaxLength(150)]
        public required string LastName { get; set; }

        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
