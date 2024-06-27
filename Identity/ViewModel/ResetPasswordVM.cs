using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModel
{
    public class ResetPasswordVM
    {
        [Display(Name ="New Password")]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Display(Name = "New Password")]
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set;}

        //Hide
        public string Email { get; set; }
        public string token { get; set; }
    }
}
