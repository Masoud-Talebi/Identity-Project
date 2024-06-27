using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModel
{
    public class RegisterVM
    {
        [Required]
        [MaxLength(150)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(150)]
        public string  LastName { get; set; }
        [Required]
        [MaxLength(250)]
        [Remote("AnyUserName","Account",HttpMethod = "Post" , AdditionalFields = "__RequestVerificationToken")]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        [Remote("AnyEmail", "Account", HttpMethod = "Post", AdditionalFields = "__RequestVerificationToken")]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string RePasword { get; set; }
    }
}
