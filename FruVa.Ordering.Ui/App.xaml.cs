using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.DataAccess;
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
                .AddSingleton<FilterWindowViewModel>()
                .AddSingleton<Context>();

            // TODO: Register log4net logger in DI
            // 1. Register ILogger as Singleton
            // 2. Add try...catch for every entry point (e.g. RelayCommands)
            // 3. Add trace logging for important steps

            Services = serviceCollection.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}
