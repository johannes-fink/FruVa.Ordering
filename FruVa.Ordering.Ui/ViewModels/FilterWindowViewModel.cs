using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.Ui.Models;
using FruVa.Ordering.Ui.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FruVa.Ordering.Ui.ViewModels
{
    public partial class FilterWindowViewModel : ObservableObject 
    {
        private readonly IService _apiService;

        private readonly List<IFilterItem> _articles = [];
        private readonly List<IFilterItem> _recipients = [];

        [ObservableProperty]
        private CollectionViewSource _filterItems = new();

        [ObservableProperty]
        private string _title = "Give me an Article";

        [ObservableProperty]
        private DataGridSelectionMode _selectionMode = DataGridSelectionMode.Extended;

        [ObservableProperty]
        public List<IFilterItem> _selectedOrderDetails = [];
        partial void OnSelectedOrderDetailsChanged(List<IFilterItem>? oldValue, List<IFilterItem> newValue)
        {
            oldValue?.ForEach(x => x.IsChecked = false);
            foreach (var item in newValue)
            {
                item.IsChecked = !item.IsChecked;
            }
        }

        [ObservableProperty]
        private string? _searchValue;
        partial void OnSearchValueChanged(string? value)
        {
            FilterItems.View?.Refresh();
        }

        [ObservableProperty]
        private bool? _isArticleFilterEnabled;
        partial void OnIsArticleFilterEnabledChanged(bool? value)
        {
            FilterItems!.Source = value == true ? _articles : _recipients;
            Title = value == true ? "Give me an Article" : "Give me a Recipient";
            SelectionMode = value == true ? DataGridSelectionMode.Extended : DataGridSelectionMode.Single;
            FilterItems.View?.Refresh();
        }

        public FilterWindowViewModel(IService apiService)
        {
            _apiService = apiService;
        }


        [RelayCommand]
        public void Cancel(IClosable window)
        {
            try
            {
                window.DialogResult = false;
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        [RelayCommand]
        public void Apply(IClosable window)
        {
            try
            {
                window.DialogResult = true;
                window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"There was an error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal async Task LoadLookupDataAsync()
        {
            try
            {
                var getArticlesTask = _apiService.GetArticlesAsync();
                var getRecipientsTask = _apiService.GetRecipientsAsync();

                await Task.WhenAll(getArticlesTask, getRecipientsTask);

                foreach (var article in getArticlesTask.Result)
                {
                    _articles.Add(new Article(article));
                }

                foreach (var recipient in getRecipientsTask.Result)
                {
                    _recipients.Add(new Recipient(recipient));
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    FilterItems!.Source = IsArticleFilterEnabled == true ? _articles : _recipients;
                    FilterItems.Filter += Filter;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"there was an error: {ex.Message}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchValue))
                {
                    e.Accepted = true;
                    return;
                }

                if (e.Item is not IFilterItem item)
                {
                    e.Accepted = false;
                    return;
                }

                if (string.IsNullOrWhiteSpace(item.SearchContent))
                {
                    e.Accepted = false;
                    return;
                }

                var searchPartials = SearchValue.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                foreach (var value in searchPartials)
                {
                    if (!item.SearchContent.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        e.Accepted = false;
                        return;
                    }
                }

                e.Accepted = true;
            }
            catch (Exception ex)
            {
                e.Accepted = false;
                MessageBox.Show($"An error occurred while filtering:\n{ex.Message}", "filtering-error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
