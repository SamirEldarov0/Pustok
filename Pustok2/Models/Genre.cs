using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [StringLength(maximumLength:25,ErrorMessage ="The length can't be more than 25")]
        [DisplayName("Genre Name")]
        public string Name { get; set; }
        public List<Book> Books { get; set; }

    }
}
