using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Windows;
using TextureCoordsCalculatorGUI.Misc;
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
        const string ListfileUri = "https://github.com/wowdev/wow-listfile/releases/latest/download/community-listfile.csv";
        public IServiceProvider? Services { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await LoadListfile();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            Services = serviceCollection.BuildServiceProvider();

            Ioc.Default.ConfigureServices(Services);

            var mainWindow = Services.GetRequiredService<MainWindow>();
   

            mainWindow.DataContext = Services.GetRequiredService<MainViewModel>();


            mainWindow.Show();

            
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<WagoService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
       
        }


        private static async Task LoadListfile()
        {
            var client = new HttpClient();

            var listfileStream = await client.GetStreamAsync(ListfileUri);

            if (listfileStream is not null)
            {
                Listfile.Instance.Initialize(listfileStream);
            }
      
        }
    }

}
