using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Pustok2.Areas.Manage.ViewModels
{
    public class CreateAdminViewModel
    {
        [StringLength(maximumLength:30)]
        public string FullName { get; set; }
        [Required]
        [StringLength(maximumLength:30)]
        public string UserName { get; set; }
        [Required]
        [StringLength(maximumLength:100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [StringLength(maximumLength:100)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string RepeatPassword { get; set; }
    }
}
