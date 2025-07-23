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
        private const string MessageBoxText = "An error occurred while deleting the order:";
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
            catch
            {
                MessageBox.Show(
                    "An error occurred while adding the order. Please try Again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to add a new order");
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
            catch
            {
                MessageBox.Show(
                    "An error occurred while closing the window Please try Again",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                _logger.Error("An error occurred while trying to close the window");
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
            catch
            {
                MessageBox.Show(
                    "An error occurred while closing the window Please try again Please try Again:",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to Delete the order");
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
            catch
            {
                MessageBox.Show(
                    "Error deleting order details Please try Again:",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to Deleting the Orderdetails");
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
            catch
            {
                MessageBox.Show(
                    "Error finding recipients Please try Agian:",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to find the recipient");
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
            catch
            {
                MessageBox.Show(
                    "An error occurred while adding articles Please try Again:",
                    "Article Selection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to add the articles");

            }
        }


        [RelayCommand]
        private void Save()
        {
            try
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
            catch
            {
                MessageBox.Show(
                    "An error occurred while saving orders Please try Again:",
                    "Save Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to save the order");
            }
        }

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
                    foreach (var detail in order.OrderDetails.OrderBy(x => x.Article.DisplayName))
                    {
                        sb.AppendLine($"{order.OrderNumber};{order.Recipient.DisplayName};{detail.Quantity};{detail.Price};{detail.Quantity * detail.Price};{detail.Article.DisplayName}");
                    }
                }

                File.WriteAllText(saveFileDialog.FileName, sb.ToString(), Encoding.UTF8);

                MessageBox.Show("CSV successfully created!", "Export finished", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch 
            {
                MessageBox.Show(
                    "An error occurred while exporting the CSV file Please try Again:",
                    "Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to find the recipient");
            }
        }


        public async Task InitializeDataAsync()
        {
            try
            {
                IsBusy = true;

                await LoadLookupDataAsync();
                await LoadOrdersAsync();
            }
            catch
            {
                MessageBox.Show(
                    "An error occurred while initializing data Please try Again :",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to initializing the data");
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task LoadLookupDataAsync()
        {
            try
            {
                var getArticlesTask = _apiService.GetArticlesAsync();
                var getRecipientsTask = _apiService.GetRecipientsAsync();

                await Task.WhenAll(getArticlesTask, getRecipientsTask);
            }
            catch
            {
                MessageBox.Show(
                    "An error occurred while doing the data Pleas try Again :",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to doing  the data ");
            }
        }

        private async Task LoadOrdersAsync()
        {
            try
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
            catch
            {
                MessageBox.Show(
                    "An error occurred while loading orders Please Try Again:",
                    "Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to loading the orders ");
            }
        }



        private async Task<List<OrderDetail>> ConvertToOrderDetailAsync(List<DataAccess.Models.OrderDetail> orderDetails)
        {
            var output = new List<OrderDetail>();

            try
            {
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
                    catch (Exception innerEx)
                    {
                        MessageBox.Show(
                            "Error processing OrderDetail with ArticleId Please try Again",
                            "OrderDetail Conversion Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        _logger.Error("An error occurred while trying to processing the orderDetail with ArticleId ",innerEx);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while converting order details Please try Again :",
                    "Conversion Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                _logger.Error("An error occurred while trying to converting the orderDetail ",ex);

            }

            return output;
        }
    }
}