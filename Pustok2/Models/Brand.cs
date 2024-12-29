using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
	public class Brand
	{
        public int Id { get; set; }
        [StringLength(maximumLength:40)]
        [Required]
        public string Img { get; set; }
        public int Order { get; set; }
    }
}
