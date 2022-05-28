using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Collections.Generic;


namespace HelpDeskBroadcaster
{
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServiceDI(services);
            serviceProvider = services.BuildServiceProvider();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWind = (MainWindow)serviceProvider.GetService(typeof(MainWindow));
            mainWind.Show();
        }

        private void ConfigureServiceDI(ServiceCollection colllection)
        {
            colllection.AddSingleton<MainWindow>();
            colllection.AddSingleton<BroadcasterBot>();
            colllection.AddSingleton<RootService>();
            colllection.AddSingleton<List<OrganizationWithUsers>>();
        }
    }
}
