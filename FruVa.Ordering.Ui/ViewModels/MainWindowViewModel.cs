using System.Buffers;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.ApiAccess.Models;
using FruVa.Ordering.DataAccess;
using FruVa.Ordering.Ui.Models;
using FruVa.Ordering.Ui.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FruVa.Ordering.Ui.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IServiceProvider _services;
        private readonly IService _apiService;
        private readonly Context _context;

        public MainWindowViewModel(Context context, IServiceProvider services, IService apiService)
        {
            _context = context;
            _services = services;
            _apiService = apiService;
        }

        // TODO: Load Orders from Database
        // 1. Get all orders with children
        // 2. Convert DB Order to UI Order

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
                    var isAlreadyInList = SelectedOrder!.OrderDetails.FirstOrDefault(x =>
                        x.Article.Id == article.Id
                    );
                    if (isAlreadyInList != null)
                    {
                        continue;
                    }

                    SelectedOrder!.OrderDetails.Add(
                        new OrderDetail { Article = (Models.Article)article }
                    );
                }
            }
        }

        [RelayCommand]
        private void Save()
        {
            foreach (var order in Orders)
            {
                var newDbOrder = new DataAccess.Models.Order()
                {
                    OrderNumber = order.OrderNumber,
                    RecipientId = order.Recipient.Id!.Value,
                };

                foreach (var uiOrderDetail in order.OrderDetails)
                {
                    var newOrderDetail = new DataAccess.Models.OrderDetail()
                    {
                        Quantity = uiOrderDetail.Quantity ?? 0,
                        Price = uiOrderDetail.Price ?? 0,
                        ArticleId = uiOrderDetail.Article.Id!.Value,
                        Order = newDbOrder,
                    };
                    newDbOrder.OrderDetails.Add(newOrderDetail);
                }

                _context.Orders.Add(newDbOrder);
            }

            _context.SaveChanges();
        }

        public async Task LoadOrdersAsync()
        {
            Orders.Clear();
            var dbOrders = _context.Orders
                .Include(o => o.OrderDetails)
                .ToList();

            foreach (var dbOrder in dbOrders)
            {
                var recipient = await _apiService.GetRecipientByIdAsync(dbOrder.RecipientId);
                if (recipient == null)
                {
                    continue;
                }

                var orderDetails = await ConvertToOrderDetailAsync(dbOrder.OrderDetails);

                var uiOrder = new Order
                {
                    OrderNumber = dbOrder.OrderNumber,
                    Recipient = new Models.Recipient(recipient),
                    OrderDetails = [.. orderDetails]
                };

                App.Current.Dispatcher.Invoke(() =>
                {
                    Orders.Add(uiOrder);
                });
            }
        }

        private async Task<List<OrderDetail>> ConvertToOrderDetailAsync(List<DataAccess.Models.OrderDetail> orderDetails)
        {
            var output = new List<OrderDetail>();
            foreach (var orderDetail in orderDetails)
            {
                var apiArticle = await _apiService.GetArticleByIdAsync(orderDetail.ArticleId);
                if (apiArticle == null)
                {
                    continue;
                }

                output.Add(new OrderDetail
                {
                    Quantity = orderDetail.Quantity,
                    Price = orderDetail.Price,
                    Article = new Models.Article(apiArticle)
                });
            }

            return output;
        }
    }
}
