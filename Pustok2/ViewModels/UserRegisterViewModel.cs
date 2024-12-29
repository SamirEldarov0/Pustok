using System.ComponentModel.DataAnnotations;

namespace Pustok2.ViewModels
{
	public class UserRegisterViewModel
	{
        [StringLength(maximumLength:50)]
        public string FullName { get; set; }
        [Required]
        [StringLength(maximumLength:25)]
        public string UserName { get; set; }
        [Required]
        [StringLength(maximumLength:100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength:20)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [StringLength(maximumLength:20)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))] //Password ve ConfirmPassword muqayisesi
        public string ConfirmPassword { get; set; }
    }
}
