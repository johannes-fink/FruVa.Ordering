using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace FruVa.Ordering.Ui.Models
{
    public partial class Order : ObservableObject
    {
        public decimal? TotalPrice => OrderDetails.Sum(x => x.Price);

        [ObservableProperty]
        public int _orderNumber;

        [ObservableProperty]
        private Recipient _recipient;
        
        [ObservableProperty]
        public ObservableCollection<OrderDetail> _orderDetails = [];
    }
}