using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok2.Models
{
    public class SliderRevision
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 100)]
        public string Img { get; set; }
        [StringLength(maximumLength: 100)]
        public string Title { get; set; }
        [StringLength(maximumLength: 100)]
        public string Subtitle { get; set; }
        [StringLength(maximumLength: 100)]
        [DisplayName("Slider-Name")]
        public string PriceTxt { get; set; }
        public string RedirectURL { get; set; }
        public int Order { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
