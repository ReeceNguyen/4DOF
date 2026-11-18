using System;
namespace prototype_mqtt.Models;

public class DatabaseConfig
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 5432;
    public string Username { get; set; } = "opta";
    public string Password { get; set; } = "s7200smart";
    public string Database { get; set; } = "prototype_mqtt";

    public string GetConnectionString() =>
        $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Database};";
}
public class LedStateRecord
{
    public int LedId { get; set; }
    public string LedName { get; set; } = string.Empty;
    public bool IsOn { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class SensorReadingRecord
{
    public int SensorId { get; set; }
    public string SensorName { get; set; } = string.Empty;
    public double ActualValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
}

public class SensorSettingRecord
{
    public int SensorId { get; set; }
    public string SensorName { get; set; } = string.Empty;
    public double MinThreshold { get; set; }
    public double MaxThreshold { get; set; }
    public bool IsEnabled { get; set; }
}