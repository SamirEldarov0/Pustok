using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class Feature
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:100)]
        public string Icon { get; set; }
        [StringLength(maximumLength:100)]
        public string Title { get; set; }
        [StringLength(maximumLength:110)]
        public string Subtitle { get; set; }
        public int Order { get; set; }

    }
}
