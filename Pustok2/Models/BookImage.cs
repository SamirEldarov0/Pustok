using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class BookImage
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        [StringLength(maximumLength:100)]
        public string Image { get; set; }
        public bool? PosterStatus { get; set; }
        public Book Book { get; set; }
    }
}
