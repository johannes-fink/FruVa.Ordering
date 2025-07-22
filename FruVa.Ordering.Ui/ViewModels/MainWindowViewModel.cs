using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.DataAccess;
using FruVa.Ordering.Ui.Models;
using FruVa.Ordering.Ui.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

namespace FruVa.Ordering.Ui.ViewModels
{
    public partial class MainWindowViewModel(Context context, IServiceProvider services, IService apiService) : ObservableObject
    {
        private readonly IServiceProvider _services = services;
        private readonly IService _apiService = apiService;
        private readonly Context _context = context;

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
            try
            {
                var newOrder = new Order
                {
                    OrderNumber = Orders.Count + 1,
                };

                Orders.Add(newOrder);
                SelectedOrder = newOrder;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while adding the order:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        [RelayCommand]
        private void Cancel(IClosable window)
        {
            try
            {
                window.Close();
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while closing the window:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        [RelayCommand]
        private void DeleteOrder()
        {
            try
            {
                if (SelectedOrder == null)
                {
                    return;
                }

                Orders.Remove(SelectedOrder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while deleting the order:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        [RelayCommand]
        private void DeleteOrderDetail()
        {
            try
            {
                if (SelectedOrder == null || SelectedOrderDetail == null)
                {
                    return;
                }

                SelectedOrder.OrderDetails.Remove(SelectedOrderDetail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting order details:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        [RelayCommand]
        private void FindRecipients()
        {
            try
            {
                var filterWindow = _services.GetRequiredService<FilterWindow>();
                filterWindow.IsArticleFilterEnabled = false;
                filterWindow.ShowDialog();

                if (filterWindow.SelectedItems.Count != 0)
                {
                    SelectedOrder!.Recipient = (Models.Recipient)filterWindow.SelectedItems[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error finding recipients:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        [RelayCommand]
        private void FindArticles()
        {
            var filterWindow = _services.GetRequiredService<FilterWindow>();
            filterWindow.IsArticleFilterEnabled = true;
            filterWindow.ShowDialog();

            if (filterWindow.SelectedItems.Count != 0)
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

        [RelayCommand]
        private void ExportToCsv()
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "orders.csv",
                Filter = "CSV-Datei (*.csv)|*.csv"
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result != true)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("No;Recipient;Quantity;Price;Total;Article");

            foreach (var order in Orders.OrderBy(x => x.OrderNumber).ToList())
            {
                foreach (var detail in order.OrderDetails.OrderBy(x => x.Article.DisplayName))
                {
                    sb.AppendLine($"{order.OrderNumber};{order.Recipient.DisplayName};{detail.Quantity};{detail.Price};{detail.Quantity * detail.Price};{detail.Article.DisplayName}");
                }
            }

            File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

            MessageBox.Show("CSV successfully created!", "Export finished", MessageBoxButton.OK, MessageBoxImage.Information);
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
