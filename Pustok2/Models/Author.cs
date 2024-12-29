using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok2.Models
{
    public class Author
    {
        public int Id { get; set; }
        [StringLength(maximumLength:50)]
        public string Name { get; set; }
        [StringLength(maximumLength:600)]
        public string Desc { get; set; }
        [StringLength(maximumLength:90)]
        public string Img { get; set; }
        [NotMapped]  //Bu propertinin sql-le elaqesi yoxdur
        public IFormFile ImageFile { get; set; }
        public List<Book> Books { get; set; }
    }
}
