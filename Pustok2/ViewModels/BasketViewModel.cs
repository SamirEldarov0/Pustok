using System.Collections.Generic;

namespace Pustok2.ViewModels
{
    public class BasketViewModel
    {
        public List<BasketItemViewModel> BasketViewItems { get; set; }
        public int Count { get; set; }
        public double TotalPrice { get; set; }

    }
}
