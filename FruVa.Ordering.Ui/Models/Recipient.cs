using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FruVa.Ordering.Ui.Models
{
    public partial class Recipient : ObservableObject, IFilterItem
    {
        public Recipient(ApiAccess.Models.Recipient sourceItem)
        {
            SourceItem = sourceItem;
            DisplayName = $"{SourceItem.Name} ({SourceItem.Street} | {SourceItem.StreetNumber} | {SourceItem.Country})";
        }

        public ApiAccess.Models.Recipient? SourceItem { get; set; }

        public Guid? Id => SourceItem?.Id;
        public string? DisplayName { get; set; }

        [ObservableProperty]
        private bool _isChecked = false;

        public string? SearchContent => SourceItem?.ToString();

    }
}
