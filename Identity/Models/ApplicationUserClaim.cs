using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
