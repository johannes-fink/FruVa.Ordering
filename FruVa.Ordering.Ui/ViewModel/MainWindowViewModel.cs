using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FruVa.Ordering.Ui.Model;
using System.Collections.ObjectModel;

namespace FruVa.Ordering.Ui.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            Orders = new ObservableCollection<Order>
            {
                new Order 
                { 
                    OrderNumber = 1, 
                    RecipientName = "Test1", 
                    OrderDetails =
                    [
                        new OrderDetail { ArticleName = "Artikel 1", Quantity = 1, Price = 10m },
                        new OrderDetail { ArticleName = "Artikel 2", Quantity = 2, Price = 20m }
                    ] 
                },
                new Order { OrderNumber = 2, RecipientName = "Test2" }
            };
        }

        [ObservableProperty]
        private ObservableCollection<Order> _orders = [];

        [ObservableProperty]
        private Order? _selectedOrder;

        [ObservableProperty]
        private OrderDetail? _selectedOrderDetail;


        [RelayCommand]
        private void AddOrder()
        {
            var newOrder = new Order
            {
                OrderNumber = Orders.Count + 1,
                RecipientName = "Neuer Empfänger",
                OrderDetails =
                [
                    new OrderDetail { ArticleName = "Artikel 1", Quantity = 1, Price = 10m },
                    new OrderDetail { ArticleName = "Artikel 2", Quantity = 2, Price = 20m }
                ]
            };

            Orders.Add(newOrder);
            SelectedOrder = newOrder;
        }

        [RelayCommand]
        private void Cancel() 
        { 
            App.Current.Shutdown();
        }

        [RelayCommand]
        private void DeleteOrder()
        {
            if (SelectedOrder == null)
            {
                return;
            }

            Orders.Remove(SelectedOrder);
        }

        [RelayCommand]
        private void DeleteOrderDetail()
        {
            if (SelectedOrder == null || SelectedOrderDetail == null)
            {
                return;
            }

            SelectedOrder.OrderDetails.Remove(SelectedOrderDetail);
        }

        [RelayCommand]
        private void FindRecipient() => throw new NotImplementedException();

        [RelayCommand]
        private void FindArticle() => throw new NotImplementedException();
    }
}



