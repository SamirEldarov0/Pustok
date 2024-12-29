using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class Publisher
    {
        public int Id { get; set; }
        [StringLength(maximumLength:30)]
        public string Name { get; set; }
        public List<Book> Books { get; set; }

    }
}
