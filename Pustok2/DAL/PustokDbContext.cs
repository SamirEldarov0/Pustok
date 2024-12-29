using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pustok2.Models;

namespace Pustok2.DAL
{
    public class PustokDbContext:IdentityDbContext
    {
        public PustokDbContext(DbContextOptions<PustokDbContext> options):base(options)
        {
            
        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<UpPromotion> UpPromotions { get; set; }
        public DbSet<DownPromotion> DownPromotions { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<Brand2> Brand2s { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<SliderRevision> SliderRevisions { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
		public DbSet<BookCampaign> BookCampaigns { get; set; }

	}
}
