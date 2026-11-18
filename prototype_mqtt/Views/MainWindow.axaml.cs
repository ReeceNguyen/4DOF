using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Avalonia;
namespace prototype_mqtt.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private async void SaveBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        // Publish the typed value via MQTT and persist to file
        if (DataContext is prototype_mqtt.ViewModels.MainViewModel vm)
        {
            await vm.SaveChannelAsync(vm.PressureChannel, this.TxbxSetPressure.Text ?? string.Empty);
            await vm.SaveChannelAsync(vm.TemperatureChannel,this.TxbxSetTemperature.Text??string.Empty);
        }

        // Disable editing after save
        this.TxbxSetPressure.IsEnabled = false;
        this.TxbxSetTemperature.IsEnabled = false;
        this.BtnSave.IsEnabled = false;
    }

    private async void EditBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        var box = MessageBoxManager.GetMessageBoxStandard(
            "Parameters Edit",
            "Do you want to change?",
            ButtonEnum.YesNo);

        var result = await box.ShowAsync();

        if (result == ButtonResult.Yes)
        {
            // Enable Set Value textbox and Save button
            this.TxbxSetPressure.IsEnabled = true;
            this.TxbxSetTemperature.IsEnabled = true;
            this.BtnSave.IsEnabled = true;
        }
        // If No: nothing changes
    }
    private void LogoutBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var loginWindow = new LoginWindow();
            desktop.MainWindow = loginWindow;
        
            loginWindow.Closed += (_, _) =>
            {
                if (loginWindow.AuthenticatedConfig != null)
                {
                    var newMain = new MainWindow
                    {
                        DataContext = new prototype_mqtt.ViewModels.MainViewModel(loginWindow.AuthenticatedConfig)
                    };
                    desktop.MainWindow = newMain;
                    desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    newMain.Show();
                }
                else
                {
                    desktop.Shutdown();
                }
            };

            this.Close();
            loginWindow.Show();
        }
    }
}