using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace FruVa.Ordering.Ui.Models
{
    public partial class Order : ObservableObject
    {
        // TODO: Calculate TotalPrice on change of order details
        // 1. React to changes in OrderDetails (Quantity or Price)
        // 2. Calculate product of each OrderDetail Quantity * Price
        // 3. Sum product of 2.
        public decimal? TotalPrice => OrderDetails.Sum(x => x.Price);

        public Recipient RecipientId { get; internal set; }

        [ObservableProperty]
        private int _orderNumber;

        [ObservableProperty]
        private Recipient _recipient;
        
        [ObservableProperty, NotifyPropertyChangedFor(nameof(TotalPrice))]
        private ObservableCollection<OrderDetail> _orderDetails = [];
    }
}