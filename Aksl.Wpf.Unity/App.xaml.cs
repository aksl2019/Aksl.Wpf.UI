using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using Aksl.Toolkit.Dialogs;
using Aksl.Toolkit.Services;

using Aksl.Infrastructure;

using Aksl.Modules.Home;
using Aksl.Modules.Shell;
using Aksl.Modules.Shell.ViewModels;
using Aksl.Modules.Shell.Views;

using Aksl.Modules.HamburgerMenuSideBar;
using Aksl.Modules.HamburgerMenuTreeSideBar;
using Aksl.Modules.HamburgerMenuTreeBar;

using Aksl.Modules.HamburgerMenu;
using Aksl.Modules.HamburgerMenuNavigationBar;
using Aksl.Modules.MenuSub;

using Aksl.Modules.HamburgerMenuNavigationSideBar;
using Aksl.Modules.Account;

using Aksl.Modules.IndustryNavigationBar;
using Aksl.Modules.Pipeline;
using Aksl.Modules.Thermometer;
using Aksl.Modules.CoolingTower;
using Aksl.Modules.AirCompresser;

namespace Aksl.Wpf.Unity
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register(typeof(ShellView).ToString(), () => Container.Resolve<ShellViewModel>());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            #region Initialize
            var services = new ServiceCollection();
            services.AddOptions();

            string basePath = Directory.GetCurrentDirectory();
            string configPath = Path.Combine(basePath, "Configuration");
            string appSettingsPath = Path.Combine(configPath, "appsettings.json");
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().SetBasePath(basePath)
                                                                                   .AddJsonFile(path: appSettingsPath, optional: true, reloadOnChange: false);

            var configuration = configurationBuilder.Build();
            #endregion

            #region Logging
            services.AddLogging(builder =>
            {
                var loggingSection = configuration.GetSection("Logging");
                var includeScopes = loggingSection.GetValue<bool>("IncludeScopes");

                builder.AddConfiguration(loggingSection);

                //加入一个ConsoleLoggerProvider
                //builder.AddConsole(consoleLoggerOptions =>
                //{
                //    consoleLoggerOptions.IncludeScopes = includeScopes;
                //});

                //加入一个DebugLoggerProvider
                builder.AddDebug();
            });
            #endregion

            #region SocketListenOptions
           
            #endregion

            services.AddHttpClient();
            var serviceProvider = services.BuildServiceProvider();

            containerRegistry.RegisterInstance<IServiceProvider>(serviceProvider);

            containerRegistry.RegisterSingleton(typeof(IDialogService), typeof(DialogService));
            containerRegistry.RegisterSingleton(typeof(IDialogViewService), typeof(DialogViewService));

            containerRegistry.RegisterDialog<ConfirmView, ConfirmViewModel>();

            RegisterMenuFactoryAsync(containerRegistry).GetAwaiter().GetResult();
        }

        protected async Task RegisterMenuFactoryAsync(IContainerRegistry containerRegistry)
        {
            try
            {
                MenuService menuService = new (new List<string> {"pack://application:,,,/Aksl.Wpf.Unity;Component/Data/AllMenus.xml",
                                                                 "pack://application:,,,/Aksl.Wpf.Unity;Component/Data/Industry.xml"
                                                                 });

                await menuService.CreateMenusAsync();

                containerRegistry.RegisterInstance<IMenuService>(menuService );

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            _ = moduleCatalog.AddModule(nameof(HomeModule), typeof(HomeModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(nameof(AccountModule), typeof(AccountModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(nameof(PipelineModule), typeof(PipelineModule).AssemblyQualifiedName,nameof(IndustryNavigationBarModule));
            _ = moduleCatalog.AddModule(nameof(ThermometerModule), typeof(ThermometerModule).AssemblyQualifiedName, nameof(IndustryNavigationBarModule));
            _ = moduleCatalog.AddModule(nameof(CoolingTowerModule), typeof(CoolingTowerModule).AssemblyQualifiedName, nameof(IndustryNavigationBarModule));
            _ = moduleCatalog.AddModule(nameof(AirCompresserModule), typeof(AirCompresserModule).AssemblyQualifiedName, nameof(IndustryNavigationBarModule));
            _ = moduleCatalog.AddModule(nameof(IndustryNavigationBarModule), typeof(IndustryNavigationBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(nameof(MenuSubModule), typeof(MenuSubModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(HamburgerMenuModule),typeof(HamburgerMenuModule).AssemblyQualifiedName,InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(HamburgerMenuNavigationBarModule), typeof(HamburgerMenuNavigationBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(nameof(HamburgerMenuTreeBarViewModule), typeof(HamburgerMenuTreeBarViewModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            _ = moduleCatalog.AddModule(typeof(HamburgerMenuSideBarModule).Name, typeof(HamburgerMenuSideBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
           _ = moduleCatalog.AddModule(typeof(HamburgerMenuNavigationSideBarModule).Name, typeof(HamburgerMenuNavigationSideBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(typeof(HamburgerMenuTreeSideBarViewModule).Name, typeof(HamburgerMenuTreeSideBarViewModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            _ = moduleCatalog.AddModule(typeof(ShellModule).Name, typeof(ShellModule).AssemblyQualifiedName, InitializationMode.WhenAvailable, typeof(HamburgerMenuSideBarModule).Name);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override  void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}
