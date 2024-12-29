using System.ComponentModel.DataAnnotations;

namespace Pustok2.ViewModels
{
	public class UserLogin2ViewModel
	{
        [StringLength(maximumLength:30)]
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(maximumLength:50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
