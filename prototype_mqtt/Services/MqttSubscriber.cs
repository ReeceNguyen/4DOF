using System.Threading.Tasks;
using System.Text;
using System;
using MQTTnet;
namespace prototype_mqtt.Services;

public class MqttSubscriber : MqttPublisher
{
    /// <summary>
    /// Generic event fired for every incoming MQTT message: (topic, rawPayload).
    /// Used by MainViewModel to route messages to both digital channels (LEDs, relays)
    /// and analog/parameter channels (temperature, pressure, etc.).
    /// </summary>
    public event Action<string, string>? OnTopicMessageReceived;



    public MqttSubscriber() : this(new MqttConfig()) { }
    public MqttSubscriber(IMqttClient client, MqttConfig config) : base(client, config) { SetupHandler(); }
    public MqttSubscriber(MqttConfig config) : base(config) { SetupHandler(); }

    private void SetupHandler()
    {
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic   = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            // Generic routing for any device
            OnTopicMessageReceived?.Invoke(topic, payload);



            return Task.CompletedTask;
        };
    }

    /// <summary>Generic subscribe method for any device topic.</summary>
    public async Task SubscribeTopicAsync(string topic)
    {
        if (_mqttClient != null && _mqttClient.IsConnected)
            await _mqttClient.SubscribeAsync(topic);
    }
}