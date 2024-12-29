using Pustok2.Models;
using System.ComponentModel.DataAnnotations;
using System;

namespace Pustok2.ViewModels
{
    public class CommentCreateViewModel
    {
        public int BookId { get; set; }
        [StringLength(maximumLength: 250)]
        public string Text { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }
    }
}
