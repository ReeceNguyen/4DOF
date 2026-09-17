using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using prototype_mqtt.Models;

namespace prototype_mqtt.ViewModels;

/// <summary>
/// Reusable ViewModel representing one digital ON/OFF device (LED, Relay, Valve, Fan, etc.).
/// Provides real-time status binding (IsOn, StatusColor, StatusText) and an asynchronous ToggleCommand.
/// </summary>
public partial class DigitalChannelViewModel : ObservableObject
{
    private readonly DigitalDevice _config;
    private readonly Func<string, string, Task> _publishFunc;
    private readonly Func<Task> _saveStateFunc;

    public string Name => _config.Name;
    public string Topic => _config.Topic;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusColor))]
    [NotifyPropertyChangedFor(nameof(StatusText))]
    private bool _isOn;

    public string StatusColor => IsOn ? _config.OnColor : _config.OffColor;
    public string StatusText  => IsOn ? _config.OnText  : _config.OffText;

    public DigitalChannelViewModel(
        DigitalDevice config,
        Func<string, string, Task> publishFunc,
        Func<Task> saveStateFunc)
    {
        _config = config;
        _publishFunc = publishFunc;
        _saveStateFunc = saveStateFunc;
    }

    /// <summary>
    /// Updates IsOn from an incoming MQTT payload ("1", "0", "true", "false", etc.).
    /// </summary>
    public void UpdateFromPayload(string payload)
    {
        IsOn = payload == _config.OnPayload ||
               payload.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               payload == "1";
    }

    /// <summary>
    /// Toggles state, publishes the new payload to the MQTT topic, and saves state to file.
    /// </summary>
    [RelayCommand]
    public async Task ToggleAsync()
    {
        var newState = !IsOn;
        IsOn = newState;
        var payload = newState ? _config.OnPayload : _config.OffPayload;

        await _publishFunc(_config.Topic, payload);
        await _saveStateFunc();
    }
}