using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Pustok2.Models
{
    public class Book
    {
        public int Id { get; set; }
        //public int TagId { get; set; }
        public int GenreId { get; set; }
        public int AuthorId { get; set; }
        public int PublisherId { get; set; }
        [StringLength(maximumLength:70)]
        public string Name { get; set; }
        public double ProducingPrice { get; set; }
        public double Price { get; set; }
        public double DiscountedPrice { get; set; }
        public int Rate { get; set; }
        [Required]
        [StringLength(maximumLength:20)]
        public string Code { get; set; }
        [StringLength(maximumLength:1500)]
        public string Desc { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsNew { get; set; }
        public bool IsFeatured { get; set; }
        //[Required]-editd-de sekil daxil olmaya biler...
        [NotMapped]
        //Pokster Image Name-de ola biler
        public int PosterId { get; set; }
        [NotMapped]
        public List<int> ImageIds { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }
        [NotMapped]
        public IFormFile PosterImageFile { get; set; }
        //[Required]
        [NotMapped]
        public IFormFile HoverPosterImageFile { get; set; }
        [NotMapped]
        public List<IFormFile> Images { get; set; }

        public Genre Genre { get; set; }
        public Author Author { get; set; }
        public Publisher Publisher { get; set; }
        //public Tag Tag { get; set; }
        public List<BookImage> BookImages { get; set; }
        //public List<Tag> Tags { get; set; }
        public List<BookTag> BookTags { get; set; }
        public List<Comment> Comments { get; set; }
        public List<AppUser> AppUsers { get; set; }
        public List<BasketItem> BasketItems { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public List<BookCampaign> BookCampaigns { get; set; }


    }
}
