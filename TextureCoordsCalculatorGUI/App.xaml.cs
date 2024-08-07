using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TextureCoordsCalculatorGUI.Services;
using TextureCoordsCalculatorGUI.Shared;
using TextureCoordsCalculatorGUI.ViewModels;

namespace TextureCoordsCalculatorGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            Services = serviceCollection.BuildServiceProvider();

            Ioc.Default.ConfigureServices(Services);

            var mainWindow = Services.GetRequiredService<MainWindow>();
            
            mainWindow.DataContext = Services.GetRequiredService<MainViewModel>();
            mainWindow.Show();
            
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<WagoService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<Area>();
       
        }
    }

}
