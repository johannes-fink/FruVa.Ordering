using CommunityToolkit.Mvvm.ComponentModel;
using FruVa.Ordering.ApiAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruVa.Ordering.Ui.ViewModels
{
    public partial class FilterWindowViewModel : ObservableObject
    {
        private readonly IService _apiService;

        [ObservableProperty]
        private ObservableCollection<ApiAccess.Models.Article> _articles = [];

        [ObservableProperty]
        private ObservableCollection<ApiAccess.Models.Recipient> _recipients = [];

        public FilterWindowViewModel(IService apiService)
        {
            _apiService = apiService;
        }

        internal async Task LoadLookupDataAsync()
        {
            var getArticlesTask = _apiService.GetArticlesAsync();
            var getRecipientsTask = _apiService.GetRecipientsAsync();

            await Task.WhenAll(getArticlesTask, getRecipientsTask).ContinueWith((x) =>
            {
                Articles = [.. getArticlesTask.Result];
                Recipients = [.. getRecipientsTask.Result];
            });
        }

        private List<ApiAccess.Models.Article>? FindArticles(string searchValue)
        {
            var searchPartials = searchValue.Split(" ");

            var result = Articles.ToList();
            foreach (var value in searchPartials)
            {
                result = result.Where(x =>
                    x.ToString().Contains(value, StringComparison.InvariantCultureIgnoreCase)
                ).ToList();
            }

            return result;
        }

        private ApiAccess.Models.Recipient FindRecipients(string searchValue)
        {
            throw new NotImplementedException();
        }
    }
}
