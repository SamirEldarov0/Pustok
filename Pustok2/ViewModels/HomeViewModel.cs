using Pustok2.Models;
using System.Collections.Generic;

namespace Pustok2.ViewModels
{
    public class HomeViewModel
    {
        public List<Slider> Sliders { get; set; }
        public List<Feature> Features { get; set; } 
        public List<UpPromotion> UpPromotions { get; set; }
        public DownPromotion DownPromotion { get; set; }
        public List<BookImage> BookImages { get; set; }
        public List<Book> FeaturedBooks { get; set; }
        public List<Book> NewBooks { get; set; }
        public List<Book> AvailableBooks { get; set; }
        public Setting Setting { get; set; }


    }
}
