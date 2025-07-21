using CommunityToolkit.Mvvm.Input;
using FruVa.Ordering.Ui.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FruVa.Ordering.Ui.Views
{
    public partial class FilterWindow : Window, IClosable
    {
        private readonly FilterWindowViewModel _vm;

        internal bool? IsArticleFilterEnabled 
        { 
            get
            {
                return _vm.IsArticleFilterEnabled;
            }
            set
            {
                _vm.IsArticleFilterEnabled = value;
            }
        }

        public FilterWindow(FilterWindowViewModel viewModel)
        {
            _vm = viewModel;
            DataContext = viewModel;

            InitializeComponent();
        }

        protected override async void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            await _vm.LoadLookupDataAsync();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}
