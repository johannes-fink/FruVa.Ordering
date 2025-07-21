using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruVa.Ordering.Ui.Models
{
    public partial class Article : ObservableObject, IFilterItem
    {
        public Article()
        {
            
        }

        public Article(ApiAccess.Models.Article sourceItem)
        {
            SourceItem = sourceItem;
            DisplayName = $"{SourceItem.ArticleName} ({SourceItem.PackageSize} | {SourceItem.Caliber} | {SourceItem.OriginCountry})";
        }

        public ApiAccess.Models.Article? SourceItem { get; set; }

        public Guid? Id => SourceItem?.Id;
        public string? DisplayName { get; set; }

        [ObservableProperty]
        private bool _isChecked = false;

        public string? SearchContent => SourceItem?.ToString();
    }
}
