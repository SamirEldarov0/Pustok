using System.ComponentModel.DataAnnotations;

namespace Pustok2.Areas.Manage.ViewModels
{
    public class AdminUpdateViewModel
    {
        [Required]
        public string AdminId { get; set; }
        [StringLength(maximumLength:30)]
        public string FullName { get; set; }
        [Required]
        [StringLength(maximumLength:30)]
        public string UserName { get; set; }
        [Required]
        [StringLength(maximumLength:50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength:25)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [StringLength(maximumLength: 25)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [Required]
        [StringLength(maximumLength: 25)]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }


    }
}
