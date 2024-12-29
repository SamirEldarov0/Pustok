using System.ComponentModel.DataAnnotations;

namespace Pustok2.ViewModels
{
	public class UserLoginViewModel
	{
		[Required]
		[StringLength(maximumLength: 25,ErrorMessage = "The length of username should be less than 25")]
		public string UserName { get; set; }
		[Required]
		[StringLength(maximumLength: 20)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

    }
}
