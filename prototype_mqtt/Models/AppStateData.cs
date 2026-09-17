using System;
using System.Collections.Generic;
namespace prototype_mqtt.Models;

public class AppStateData
{
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    /// <summary>
    /// Stores the last known state for all digital ON/OFF devices (LEDs, Relays, etc.).
    /// Key   = DigitalDevice.Name (e.g. "Opta LED", "Relay 1")
    /// Value = bool state (true = ON, false = OFF)
    /// </summary>
    public Dictionary<string, bool> DigitalStates { get; set; } = new();

    /// <summary>
    /// Legacy property preserved for backward compatibility with existing JSON files.
    /// Maps to DigitalStates["Opta LED"].
    /// </summary>
    public bool IsLedOn
    {
        get => DigitalStates.TryGetValue("Opta LED", out var val) ? val : _isLedOnLegacy;
        set
        {
            _isLedOnLegacy = value;
            DigitalStates["Opta LED"] = value;
        }
    }
    private bool _isLedOnLegacy = false;

    /// <summary>
    /// Stores the last user-written set value per parameter channel.
    /// Key   = DeviceParameter.Name  (e.g. "Pressure", "Temperature")
    /// Value = last SetValue the user saved for that channel
    /// CurrentValue is excluded — it is always live from the broker.
    /// </summary>
    public Dictionary<string, string> ParameterSetValues { get; set; } = new();

    public double SetTem1 { get; set; } = 0;
    public double CurrentTem1 { get; set; } = 0;
}