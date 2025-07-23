using FruVa.Ordering.Ui.Converters;
using FruVa.Ordering.Ui.ViewModels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FruVa.Ordering.Ui.Views
{
    public partial class MainWindow : Window, IClosable
    {
        private MainWindowViewModel? _vm;

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