using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok2.ViewModels
{
	public class UserRegister2ViewModel
	{
        [StringLength(maximumLength:30)]
        public string FullName { get; set; }
        [Required]
		[StringLength(maximumLength: 30)]
		public string UserName { get; set; }
        [Required]
        [StringLength(maximumLength:40)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
		[Required]
		[StringLength(maximumLength: 40)]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Required]
		[StringLength(maximumLength: 40)]
		[DataType(DataType.Password)]
		[Compare(nameof(Password))]
		public string ConfirmPassword { get; set; }
    }
}
