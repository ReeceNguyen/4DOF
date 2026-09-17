namespace prototype_mqtt.Models;

/// <summary>
/// Configuration for a digital ON/OFF device (LED, Relay, Valve, Pump, Digital Output, etc.).
/// Add a new DigitalDevice instance whenever you have a new digital device on the PLC or broker.
/// </summary>
public class DigitalDevice
{
    /// <summary>Unique name used as the persistence key in app_state.json (e.g. "Opta LED", "Relay 1", "Cooling Fan").</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>MQTT topic for command and status feedback (e.g. "opta/led/status", "opta/relay1/state").</summary>
    public string Topic { get; init; } = string.Empty;

    /// <summary>Payload value representing ON (default "1").</summary>
    public string OnPayload { get; init; } = "1";

    /// <summary>Payload value representing OFF (default "0").</summary>
    public string OffPayload { get; init; } = "0";

    /// <summary>Color representation when ON (default "Green").</summary>
    public string OnColor { get; init; } = "Green";

    /// <summary>Color representation when OFF (default "Red").</summary>
    public string OffColor { get; init; } = "Red";

    /// <summary>Human-readable text when ON.</summary>
    public string OnText { get; init; } = "Trạng thái: Đang BẬT (1)";

    /// <summary>Human-readable text when OFF.</summary>
    public string OffText { get; init; } = "Trạng thái: Đang TẮT (0)";
}