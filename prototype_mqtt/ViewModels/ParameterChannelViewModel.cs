using CommunityToolkit.Mvvm.ComponentModel;
using prototype_mqtt.Models;
namespace prototype_mqtt.ViewModels;

public partial class ParameterChannelViewModel : ObservableObject
{
    // ── Identity ─────────────────────────────────────────────────────────────
    public string Name { get; }
    public string WriteTopic { get; }
    public string ReadTopic { get; }

    // ── Bindable state ───────────────────────────────────────────────────────

    /// <summary>
    /// The value the user wants to write to the device.
    /// Persisted to app_state.json so it survives app restarts.
    /// </summary>
    [ObservableProperty]
    private string _setValue = string.Empty;

    /// <summary>
    /// The current value reported by the device in real-time.
    /// Always sourced from the broker — never restored from the JSON file.
    /// </summary>
    [ObservableProperty]
    private string _currentValue = string.Empty;

    public ParameterChannelViewModel(DeviceParameter param)
    {
        Name       = param.Name;
        WriteTopic = param.WriteTopic;
        ReadTopic  = param.ReadTopic;
    }
}