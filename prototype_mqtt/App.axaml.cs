using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using prototype_mqtt.ViewModels;
using prototype_mqtt.Views;

namespace prototype_mqtt;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var loginWindow = new LoginWindow();
            desktop.MainWindow = loginWindow;

            loginWindow.Closed += (_, _) =>
            {
                if (loginWindow.AuthenticatedConfig != null)
                {
                    var mainWindow = new MainWindow
                    {
                        DataContext = new MainViewModel(loginWindow.AuthenticatedConfig)
                    };

                    desktop.MainWindow = mainWindow;
                    desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    mainWindow.Show();
                }
                else
                {
                    desktop.Shutdown();
                }
            };

            loginWindow.Show();
        }

        base.OnFrameworkInitializationCompleted();
    }
}