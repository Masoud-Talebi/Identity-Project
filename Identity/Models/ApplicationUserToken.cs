using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
