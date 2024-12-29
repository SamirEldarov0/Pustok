using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pustok2.Models
{
	public class Order
	{
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public DateTime OrderedDate { get; set; }
		[StringLength(maximumLength: 40)]
		public string Country { get; set; }
		[StringLength(maximumLength: 40)]
		public string City { get; set; }
		[StringLength(maximumLength: 40)]
		public string State { get; set; }
		[StringLength(maximumLength: 200)]
		public string Address { get; set; }
		[StringLength(maximumLength:200)]
        public string Note { get; set; }
        [StringLength(maximumLength: 200)]

        public string AdminNote { get; set; }
        public double TotalPrice { get; set; }
        //TotalCount elave ede bilersen
        public bool? Status { get; set; }
        public AppUser AppUser { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
