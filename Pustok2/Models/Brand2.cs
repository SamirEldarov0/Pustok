using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class Brand2
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:30)]
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
