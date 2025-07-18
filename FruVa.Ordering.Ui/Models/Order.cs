using CommunityToolkit.Mvvm.ComponentModel;
using FruVa.Ordering.Ui.ViewModel;
using System.Collections.ObjectModel;

namespace FruVa.Ordering.Ui.Models
{
    public partial class Order : ObservableObject
    {
        public decimal? TotalPrice => OrderDetails.Sum(x => x.Price);

        public int OrderNumber { get; set; }
        public required string RecipientName { get; set; }
        
        [ObservableProperty]
        public ObservableCollection<OrderDetail> _orderDetails = [];
    }
}