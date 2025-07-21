using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.Ui.Models;
using FruVa.Ordering.Ui.Views;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Data;

namespace FruVa.Ordering.Ui.ViewModels
{
    public partial class FilterWindowViewModel : ObservableObject
    {
        private readonly IService _apiService;

        private List<IFilterItem> _articles = [];
        private List<IFilterItem> _recipients = [];

        [ObservableProperty]
        private CollectionViewSource _filterItems = new CollectionViewSource();

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
            window.DialogResult = false;
            window.Close();
        }

        [RelayCommand]
        public void Apply(IClosable window)
        {
            window.DialogResult = true;
            window.Close();
        }

        internal async Task LoadLookupDataAsync()
        {
            var getArticlesTask = _apiService.GetArticlesAsync();
            var getRecipientsTask = _apiService.GetRecipientsAsync();

            await Task.WhenAll(getArticlesTask, getRecipientsTask).ContinueWith((x) =>
            {
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
            });
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchValue) == true)
            {
                e.Accepted = true;
                return;
            }

            var item = e.Item as IFilterItem;
            if (item == null)
            {
                e.Accepted = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(item.SearchContent) == true)
            {
                e.Accepted = false;
                return;
            }

            var searchPartials = SearchValue.Split(" ");
            foreach (var value in searchPartials)
            {
                e.Accepted &= item.SearchContent.Contains(value, StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}