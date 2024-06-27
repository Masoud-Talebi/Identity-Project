using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModel
{
    public class LoginVM
    {
        [DisplayName("User Name")]
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Remember Me?")]
        public bool RememberMe { get; set; }
    }
}
