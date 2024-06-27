using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModel
{
    public class FrogetPasswordVM
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
