using System;
using System.Collections.Generic;

namespace Pustok2.Models
{
	public class Campaign
	{
		public int Id { get; set; }
        public double DiscountPercent { get; set; }
        public DateTime ExpireDate { get; set; }
        public List<BookCampaign> BookCampaigns { get; set; }
    }
}
