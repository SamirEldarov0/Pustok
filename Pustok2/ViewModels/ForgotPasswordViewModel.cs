using System.ComponentModel.DataAnnotations;

namespace Pustok2.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [StringLength(maximumLength:50)]
        public string Email { get; set; }
    }
}
