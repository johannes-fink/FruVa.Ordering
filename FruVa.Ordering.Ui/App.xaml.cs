using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.Ui.ViewModels;
using FruVa.Ordering.Ui.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FruVa.Ordering.Ui
{
    public partial class App : Application
    {
        internal static IServiceProvider? Services { get; set; }

        public App()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IService, Service>()
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<FilterWindow>()
                .AddSingleton<FilterWindowViewModel>();

            Services = serviceCollection.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
