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
    public partial class FilterWindow : Window
    {
        private readonly FilterWindowViewModel _vm;

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
        public interface IClosable
        {
            void Close();
        }

        public class MainWindowViewModel
        {
            public ICommand CancelCommand { get; }

            public Action CloseAction { get; set; }

            public MainWindowViewModel()
            {
                CancelCommand = new RelayCommand(() =>
                {
                    CloseAction?.Invoke();
                });
            }
            public class RelayCommand : ICommand
            {
                private readonly Action _execute;
                private readonly Func<bool> _canExecute;

                public RelayCommand(Action execute, Func<bool> canExecute = null)
                {
                    _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                    _canExecute = canExecute;
                }

                public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

                public void Execute(object parameter) => _execute();

                public event EventHandler CanExecuteChanged
                {
                    add { CommandManager.RequerySuggested += value; }
                    remove { CommandManager.RequerySuggested -= value; }

                }

            }
        }
    }
}
