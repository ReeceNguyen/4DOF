namespace prototype_mqtt.Services;

public class MqttConfig
{
    public MqttConfig()
    {
        
    }
    public string Uri { get; set; } = "10.0.0.4";
    public string Username { get; set; } = "opta";
    public string Password { get; set; } = "s7200smart";
    public int Port { get; set; } = 1883;
    public string ClientId { get; set; } = "Opta_1";
}