using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public virtual ApplicationRole Role { get; set; }
    }
}
