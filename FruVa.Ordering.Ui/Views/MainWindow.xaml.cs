using FruVa.Ordering.Ui.ViewModels;
using System.Windows;

namespace FruVa.Ordering.Ui.Views
{
    public partial class MainWindow : Window, IClosable
    {
        private readonly MainWindowViewModel? _vm;

        public MainWindow(MainWindowViewModel viewModel)
        {
            _vm = viewModel;
            this.DataContext = viewModel;

            InitializeComponent();
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            await _vm!.InitializeDataAsync();
        }
    }
}