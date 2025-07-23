using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.RightsManagement;

namespace FruVa.Ordering.Ui.Models
{
    public partial class OrderDetail : ObservableObject
    {
        [ObservableProperty]
        private Article? _article;

        [ObservableProperty]
        public int? _quantity;

        [ObservableProperty]
        public decimal? _price;
    }
}