using System;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.ViewModels
{
    public class CommentShowViewModel
    {
        [Range(1,5)]
        public int Rate { get; set; }
        [StringLength(maximumLength:30)]
        public string FullName { get; set; }
        [StringLength(maximumLength:100)]
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}
