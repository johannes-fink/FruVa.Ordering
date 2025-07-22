using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.ApiAccess.Models;
using FruVa.Ordering.DataAccess;
using FruVa.Ordering.Ui.Models;
using FruVa.Ordering.Ui.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Buffers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Reflection;

namespace FruVa.Ordering.Ui.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _services;
        private readonly Context _context;

        public MainWindowViewModel(Context context, IServiceProvider services)
        {
            _context = context;
            _services = services;

            //Orders = new ObservableCollection<Order>
            //{
            //    new Order 
            //    { 
            //        OrderNumber = 1, 
            //        Recipient = new Models.Recipient
            //        {
            //            DisplayName = "Test1",
            //        },
            //        OrderDetails =
            //        [
            //            new OrderDetail { Article = new Models.Article { DisplayName = "Artikel 1" }, Quantity = 1, Price = 10m },
            //            new OrderDetail { Article = new Models.Article { DisplayName = "Artikel 2" }, Quantity = 2, Price = 20m }
            //        ] 
            //    },
            //    new Order { OrderNumber = 2, Recipient = new Models.Recipient { DisplayName = "Test2" } }
            //};
        }

        [ObservableProperty]
        private ObservableCollection<Order> _orders = [];

        [ObservableProperty]
        private Order? _selectedOrder;
        partial void OnSelectedOrderChanged(Order? value)
        {
            IsDetailAreaEnabled = value is not null;
        }

        [ObservableProperty]
        private OrderDetail? _selectedOrderDetail;

        [ObservableProperty]
        private bool _isDetailAreaEnabled = false;

        [RelayCommand]
        private void AddOrder()
        {
            var newOrder = new Order
            {
                OrderNumber = Orders.Count + 1,
                Recipient = new Models.Recipient { DisplayName = "Neuer Empfänger" },
                //OrderDetails =
                //[
                //    new OrderDetail { Article = new Models.Article { DisplayName = "Artikel 1" }, Quantity = 1, Price = 10m },
                //    new OrderDetail { Article = new Models.Article { DisplayName = "Artikel 2" }, Quantity = 2, Price = 20m }
                //]
            };

            Orders.Add(newOrder);
            SelectedOrder = newOrder;
        }

        [RelayCommand]
        private void Cancel(IClosable window) 
        {
            window.Close();
            Application.Current.Shutdown();
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
        private void FindRecipients()
        {
            var filterWindow = _services.GetRequiredService<FilterWindow>();
            filterWindow.IsArticleFilterEnabled = false;
            filterWindow.ShowDialog();

            if (filterWindow.SelectedItems.Any())
            {
                SelectedOrder!.Recipient = (Models.Recipient)filterWindow.SelectedItems[0];
            }
        }

        [RelayCommand]
        private void FindArticles()
        {
            var filterWindow = _services.GetRequiredService<FilterWindow>();
            filterWindow.IsArticleFilterEnabled = true;
            filterWindow.ShowDialog();

            if (filterWindow.SelectedItems.Any())
            {
                foreach (var article in filterWindow.SelectedItems)
                {
                    var isAlreadyInList = SelectedOrder!.OrderDetails.FirstOrDefault(x => x.Article.Id == article.Id);
                    if (isAlreadyInList != null)
                    {
                        continue;
                    }

                    SelectedOrder!.OrderDetails.Add(new OrderDetail { Article = (Models.Article)article });
                }
            }
        }

        [RelayCommand]
        private void Save()
        {
            // TODO: Convert UI Order model into DB model
            // Save orders to database

            foreach (var order in Orders)
            {
                var newDbOrder = new DataAccess.Models.Order()
                {
                    OrderNumber = order.OrderNumber,
                };

                foreach (var uiOrderDetail in order.OrderDetails)
                {
                    var newOrderDetail = new DataAccess.Models.OrderDetail()
                    {
                        Quantity = uiOrderDetail.Quantity ?? 0,
                    };
                }
            }
                  // context.Orders.Add(newDbOrder);
        }
    }
}




