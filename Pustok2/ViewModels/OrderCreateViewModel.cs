using Pustok2.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.ViewModels
{
	public class OrderCreateViewModel
	{
		[Required]
        [StringLength(maximumLength:40)]
        public string Country { get; set; }
		[Required]
		[StringLength(maximumLength: 40)]
		public string City { get; set; }
		[Required]
		[StringLength(maximumLength: 40)]
		public string State { get; set; }
		[Required]
		[StringLength(maximumLength: 100)]
		public string Address { get; set; }
		[StringLength(maximumLength:250)]
		public string Note { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
