using System.ComponentModel.DataAnnotations;

namespace Pustok2.Areas.Manage.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        [StringLength(maximumLength:25)]
        public string UserName { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
