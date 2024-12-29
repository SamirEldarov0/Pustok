using Pustok2.Models;

namespace Pustok2.ViewModels
{
    public class BasketItemViewModel
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public int Count { get; set; }

    }
}
