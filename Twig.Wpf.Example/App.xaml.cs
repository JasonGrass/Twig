using System.Configuration;
using System.Data;
using System.Reflection.Metadata;
using System.Windows;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Twig.Wpf.Example.Views;

namespace Twig.Wpf.Example;

public partial class App : PrismApplication
{
    private static readonly Startup AppStartup = new Startup();

    private static IServiceCollection? _serviceCollection;

    [STAThread]
    static void Main(string[] args)
    {
        using IHost host = CreateHostBuilder(args).Build();

        host.Start();

        var app = new App();
        app.InitializeComponent();
        app.Run();

        host.StopAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices(serviceCollection =>
            {
                _serviceCollection = serviceCollection;
                ConfigureServicesBeforeAppLaunch(serviceCollection);
            });

        return builder;
    }

    /// <summary>
    /// 注入 IOC 服务
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigureServicesBeforeAppLaunch(IServiceCollection services)
    {
        services.AddSingleton(_ => Current.Dispatcher);
        AppStartup.ConfigureServices(services);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        base.OnStartup(e);
    }

    protected override void OnInitialized()
    {
        AppStartup.Initialize();
        base.OnInitialized();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Exception? exception = e.ExceptionObject as Exception;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        AppStartup.RegisterView(containerRegistry);
    }

    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindowView>();
    }

    protected override void ConfigureViewModelLocator()
    {
        base.ConfigureViewModelLocator();
    }

    protected override IContainerExtension CreateContainerExtension()
    {
        if (_serviceCollection == null)
        {
            throw new InvalidOperationException(
                "Application Startup Error. Cannot found microsoft dependency injection service collection"
            );
        }

        var container = new DryIoc.Container(CreateContainerRules());
        var newContainer = container.WithDependencyInjectionAdapter(_serviceCollection);

        AppContainer.SetServiceProvider(newContainer);

        return new DryIocContainerExtension(newContainer);
    }
}
