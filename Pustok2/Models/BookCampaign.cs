namespace Pustok2.Models
{
	public class BookCampaign
	{
        public int Id { get; set; }
        public int BookId { get; set; }
        public int CampaignId { get; set; }
        public Book Book { get; set; }
        public Campaign Campaign { get; set; }
    }
}
