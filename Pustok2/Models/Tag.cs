using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class Tag
    {
        public int Id { get; set; }
        //public int BookId { get; set; }
        [StringLength(maximumLength:20)]
        public string Name { get; set; }
        //public List<Book> Books { get; set; }
        //public Book Book { get; set; }
        public List<BookTag> BookTags { get; set; }
    }
}
