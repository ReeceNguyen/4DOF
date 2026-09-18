using MQTTnet;
using System.Threading.Tasks;
namespace prototype_mqtt.Services;

public class MqttPublisher : MqttServices
{
    public MqttPublisher() : this(new MqttConfig()) { }
    public MqttPublisher(IMqttClient client, MqttConfig config) : base(client, config) { }
    public MqttPublisher(MqttConfig config) : base(config) { }

    /// <summary>
    /// Generic publish: sends <paramref name="value"/> to any <paramref name="topic"/> with Retain flag.
    /// Used by all devices (LEDs, Relays, Sensors, Transmitters).
    /// </summary>
    public async Task SendValueAsync(string topic, string value)
    {
        if (_mqttClient != null && _mqttClient.IsConnected)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(value)
                .WithRetainFlag(true)
                .Build();

            await _mqttClient.PublishAsync(message);
        }
    }


}