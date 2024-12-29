using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class UpPromotion
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:100)]
        public string Image { get; set; }
        [StringLength(maximumLength:100)]
        public string RedirectURL { get; set; }
    }
}
