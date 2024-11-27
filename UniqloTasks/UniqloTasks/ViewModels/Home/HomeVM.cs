using UniqloTasks.Models;
using UniqloTasks.ViewModels.Products;
using UniqloTasks.ViewModels.Sliders;

namespace UniqloTasks.ViewModels.Home
{
    public class HomeVM
    {
        public IEnumerable<SliderListItemVM> Sliders { get; set; }
        public IEnumerable<ProductListItemVM> Products { get; set; }
    }
}
