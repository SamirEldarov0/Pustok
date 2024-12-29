using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok2.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [StringLength(maximumLength:100)]
        public string Image { get; set; }
        [StringLength(maximumLength: 100)]
        public string Title { get; set; }
        [StringLength(maximumLength: 100)]
        public string Subtitle { get; set; }
        [StringLength(maximumLength: 60)]
        public string PriceText { get; set; }
        public string RedirectURL { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
