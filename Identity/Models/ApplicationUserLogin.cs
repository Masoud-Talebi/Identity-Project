using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
