using CommunityToolkit.Mvvm.ComponentModel;
using FruVa.Ordering.Ui.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FruVa.Ordering.Ui.Models
{
    public partial class OrderDetail : ObservableObject
    {
        [ObservableProperty]
        private Article? _article;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalPrice))]
        private int? _quantity;
        partial void OnQuantityChanged(int? value)
        {
            InvokePriceChanged();
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalPrice))]
        private decimal? _price;
        partial void OnPriceChanged(decimal? value)
        {
            InvokePriceChanged();
        }

        public decimal? TotalPrice => Quantity * Price;

        private void InvokePriceChanged()
        {
            var vm = App.Services!.GetRequiredService<MainWindowViewModel>();
            vm?.OnPriceChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}