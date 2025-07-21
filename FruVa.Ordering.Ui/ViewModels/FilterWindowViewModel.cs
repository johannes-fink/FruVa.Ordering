using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.Ui.Models;
using FruVa.Ordering.Ui.Views;
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
            FilterItems.View?.Refresh();
        }


        public FilterWindowViewModel(IService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        public void Cancel(IClosable window) => window.Close();

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