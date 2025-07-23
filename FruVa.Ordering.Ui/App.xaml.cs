using FruVa.Ordering.ApiAccess;
using FruVa.Ordering.DataAccess;
using FruVa.Ordering.Ui.Converters;
using FruVa.Ordering.Ui.ViewModels;
using FruVa.Ordering.Ui.Views;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace FruVa.Ordering.Ui
{
    public partial class App : Application
    {
        internal static IServiceProvider? Services { get; set; }

        public App()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddSingleton<IService, Service>()
                .AddSingleton<InvertBooleanConverter>()
                .AddSingleton<MainWindow>()
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<FilterWindow>()
                .AddSingleton<FilterWindowViewModel>()
                .AddSingleton<Context>()
                .AddSingleton((x) => LogManager.GetLogger(typeof(App)));

            Services = serviceCollection.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            base.OnStartup(e);
        }
    }
}
