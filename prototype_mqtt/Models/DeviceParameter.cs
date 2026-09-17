namespace prototype_mqtt.Models;

/// <summary>
/// Describes one read/write parameter channel for a device (e.g. temperature sensor, pressure transmitter).
/// Add a new instance of this class in MainViewModel to support a new device — no infrastructure changes needed.
/// </summary>
public class DeviceParameter
{
    /// <summary>Human-readable name used as the persistence key in app_state.json.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>MQTT topic the app publishes the desired set-point value to.</summary>
    public string WriteTopic { get; init; } = string.Empty;

    /// <summary>MQTT topic the device reports its actual current value on (subscribe).</summary>
    public string ReadTopic { get; init; } = string.Empty;
}