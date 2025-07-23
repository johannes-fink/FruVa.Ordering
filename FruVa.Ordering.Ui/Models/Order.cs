using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace FruVa.Ordering.Ui.Models
{
    public partial class Order : ObservableObject
    {
        [ObservableProperty]
        public decimal? _totalPrice;

        [ObservableProperty]
        private int _orderNumber;

        [ObservableProperty]
        private Recipient? _recipient;
        
        [ObservableProperty, NotifyPropertyChangedFor(nameof(TotalPrice))]
        private ObservableCollection<OrderDetail> _orderDetails = [];
        partial void OnOrderDetailsChanged(ObservableCollection<OrderDetail> value)
        {
            TotalPrice = value.Sum(x => x.TotalPrice);
        }
    }
}