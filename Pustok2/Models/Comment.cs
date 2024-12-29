using System;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        [Required]
        public string AppUserId { get; set; }
        [StringLength(maximumLength:250)]
        public string Text { get; set; }
        [Required]
        [Range(1,5)]
        public int Rate { get; set; }
        public bool? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public Book Book { get; set; }
        public AppUser AppUser { get; set; }

    }
}
