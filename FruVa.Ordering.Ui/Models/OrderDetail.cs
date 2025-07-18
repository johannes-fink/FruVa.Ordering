using CommunityToolkit.Mvvm.ComponentModel;

namespace FruVa.Ordering.Ui.Models
{
    public partial class OrderDetail : ObservableObject
    {
        [ObservableProperty]
        public string? _articleName;

        [ObservableProperty]
        public int? _quantity;

        [ObservableProperty]
        public decimal? _price;
    }
}