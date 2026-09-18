using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

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
}