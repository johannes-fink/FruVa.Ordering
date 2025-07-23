using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.ApiAccess.Models;
using FruVa.Ordering.DataAccess;
using FruVa.Ordering.Ui.Models;
using FruVa.Ordering.Ui.Views;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;

namespace FruVa.Ordering.Ui.ViewModels
{
    public partial class MainWindowViewModel(Context context, IServiceProvider services, IService apiService, ILog logger) : ObservableObject
    {
        private readonly IServiceProvider _services = services;
        private readonly IService _apiService = apiService;
        private readonly Context _context = context;
        private readonly ILog _logger = logger;

        [ObservableProperty]
        private bool _isBusy = false;

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
                    "An error occurred while adding the order. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to add a new order", ex);
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
                    "An error occurred while closing the window. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to close the window", ex);
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
                    "An error occurred while deleting the order. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to delete the order", ex);
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
                    "An error occurred while deleting an order detail. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to delete an order detail", ex);
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
                    "An error occurred while searching for a recipient. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to find a recipient", ex);
            }
        }

        [RelayCommand]
        private void FindArticles()
        {
            try
            {
                var filterWindow = _services.GetRequiredService<FilterWindow>();
                filterWindow.IsArticleFilterEnabled = true;
                filterWindow.ShowDialog();

                if (filterWindow.SelectedItems.Count != 0)
                {
                    foreach (var article in filterWindow.SelectedItems)
                    {
                        var isAlreadyInList = SelectedOrder!.OrderDetails.FirstOrDefault(x =>
                            x.Article!.Id == article.Id
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
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while searching for an article. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while searching an article.", ex);
            }
        }


        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
            try
            {
                var orderNumbers = Orders.Select(x => x.OrderNumber).ToList();
                var ordersToDelete = _context.Orders.Where(x => orderNumbers.Contains(x.OrderNumber) == false).ToList();
                _context.Orders.RemoveRange(ordersToDelete);

                foreach (var order in Orders)
                {
                    var orderFromStore = _context.Orders.FirstOrDefault(x => x.OrderNumber == order.OrderNumber);

                    if (orderFromStore is null)
                    {
                        orderFromStore = new DataAccess.Models.Order();
                        _context.Orders.Add(orderFromStore);
                    }

                    orderFromStore.OrderNumber = order.OrderNumber;
                    orderFromStore.RecipientId = order.Recipient!.Id!.Value;

                    _context.OrderDetails.RemoveRange(orderFromStore.OrderDetails);

                    foreach (var uiOrderDetail in order.OrderDetails)
                    {
                        var orderDetail = new DataAccess.Models.OrderDetail()
                        {
                            Quantity = uiOrderDetail.Quantity ?? 0,
                            Price = uiOrderDetail.Price ?? 0,
                            ArticleId = uiOrderDetail.Article!.Id!.Value,
                            Order = orderFromStore,
                        };

                        orderFromStore.OrderDetails.Add(orderDetail);
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while saving. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to save.", ex);
            }
        }

        private bool CanSave => Orders.Select(x => x.Recipient).All(x => x is not null);

        [RelayCommand]
        private void ExportToCsv()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    FileName = "orders.csv",
                    Filter = "CSV file (*.csv)|*.csv"
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
                    foreach (var detail in order.OrderDetails.OrderBy(x => x.Article!.DisplayName))
                    {
                        sb.AppendLine($"{order.OrderNumber};{order.Recipient!.DisplayName};{detail.Quantity};{detail.Price};{detail.Quantity * detail.Price};{detail.Article!.DisplayName}");
                    }
                }

                File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

                MessageBox.Show("CSV successfully created!", "Export finished", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while exporting the CSV file. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to create a CSV report.", ex);
            }
        }


        internal async Task InitializeDataAsync()
        {
            try
            {
                IsBusy = true;

                await LoadLookupDataAsync();
                await LoadOrdersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while initializing data. Please try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to initializing the data.", ex);
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task LoadLookupDataAsync()
        {
            var getArticlesTask = _apiService.GetArticlesAsync();
            var getRecipientsTask = _apiService.GetRecipientsAsync();

            await Task.WhenAll(getArticlesTask, getRecipientsTask);
        }

        private async Task LoadOrdersAsync()
        {
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
                try
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
                catch (Exception ex)
                {
                    _logger.Error("An error occurred while trying to convert an OrderDetail.", ex);
                }
            }

            return output;
        }
    }
}