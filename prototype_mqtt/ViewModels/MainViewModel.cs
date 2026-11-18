using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Threading;
using prototype_mqtt.Models;
using prototype_mqtt.Services;

namespace prototype_mqtt.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly MqttSubscriber _mqttClient;
    private readonly PostgresDataLogger _dataLogger;

    [ObservableProperty]
    private string _connectionStatus = "Đang kết nối...";

    // ═════════════════════════════════════════════════════════════════════════
    // 1. DIGITAL CHANNELS (LEDs, Relays, Valves, Switches)
    // ═════════════════════════════════════════════════════════════════════════
    public DigitalChannelViewModel OptaLedChannel { get; }

    public bool IsLedOn => OptaLedChannel.IsOn;
    public string LedColor => OptaLedChannel.StatusColor;
    public string StatusText => OptaLedChannel.StatusText;
    public IAsyncRelayCommand ToggleLedCommand => OptaLedChannel.ToggleCommand;

    private readonly List<DigitalChannelViewModel> _digitalChannels;

    // ═════════════════════════════════════════════════════════════════════════
    // 2. PARAMETER CHANNELS (Temperature, Pressure, Level, Flow, etc.)
    // ═════════════════════════════════════════════════════════════════════════
    public ParameterChannelViewModel TemperatureChannel { get; }
    public ParameterChannelViewModel PressureChannel { get; }

    private readonly List<ParameterChannelViewModel> _parameterChannels;

    // 1. Constructor mặc định (Dùng cho IDE Previewer hoặc tải cấu hình mặc định)
    public MainViewModel() : this(DatabaseConfigViewModel.LoadConfig()) { }

    // 2. Constructor chính: Nhận thông số DatabaseConfig sau khi LoginWindow xác thực thành công
    public MainViewModel(DatabaseConfig dbConfig)
    {
        // Lấy chuỗi kết nối động từ cửa sổ LoginWindow thay vì viết cứng
        _dataLogger = new PostgresDataLogger(dbConfig.GetConnectionString());
        _mqttClient = new MqttSubscriber();

        // ── Configure Digital Devices (LED, Relay, etc.) ──────────────────────
        OptaLedChannel = new DigitalChannelViewModel(
            new DigitalDevice
            {
                Name       = "Opta LED",
                Topic      = "opta/led/status",
                OnColor    = "Green",
                OffColor   = "Red",
                OnText     = "Trạng thái: Đang BẬT (1)",
                OffText    = "Trạng thái: Đang TẮT (0)"
            },
            (topic, payload) => _mqttClient.SendValueAsync(topic, payload),
            () => SaveStateAsync());

        OptaLedChannel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DigitalChannelViewModel.IsOn))
            {
                OnPropertyChanged(nameof(IsLedOn));
                OnPropertyChanged(nameof(LedColor));
                OnPropertyChanged(nameof(StatusText));
            }
        };

        _digitalChannels = [OptaLedChannel];

        // ── Configure Parameter Devices (Sensors, Transmitters) ───────────────
        TemperatureChannel = new ParameterChannelViewModel(new DeviceParameter
        {
            Name       = "Temperature",
            WriteTopic = "opta/temperature/setpoint",
            ReadTopic  = "opta/temperature/current"
        });

        PressureChannel = new ParameterChannelViewModel(new DeviceParameter
        {
            Name       = "Pressure",
            WriteTopic = "opta/pressure/setpoint",
            ReadTopic  = "opta/pressure/current"
        });

        _parameterChannels = [TemperatureChannel, PressureChannel];

        // ── Universal MQTT Message Router ────────────────────────────────────
        _mqttClient.OnTopicMessageReceived += (topic, payload) =>
        {
            // 1. Route to digital channel if topic matches
            var digitalCh = _digitalChannels.FirstOrDefault(c => c.Topic == topic);
            if (digitalCh is not null)
            {
                Dispatcher.UIThread.Post(() => digitalCh.UpdateFromPayload(payload));
                _ = SaveStateAsync();
                return;
            }

            // 2. Route to parameter channel if topic matches ReadTopic
            var paramCh = _parameterChannels.FirstOrDefault(c => c.ReadTopic == topic);
            if (paramCh is not null)
            {
                Dispatcher.UIThread.Post(() => paramCh.CurrentValue = payload);
                _ = _dataLogger.LogSensorReadingAsync(paramCh.Name, payload);
            }
        };

        _ = InitMqttAsync();
    }

    private async Task InitMqttAsync()
    {
        Dictionary<string, bool> savedDigitalStates = new();
        Dictionary<string, string> savedParamSettings = new();

        // 1. Khôi phục trạng thái gần nhất từ PostgreSQL
        try
        {
            var loaded = await _dataLogger.LoadStateAsync();
            savedDigitalStates = loaded.DigitalStates;
            savedParamSettings = loaded.ParameterSetValues;

            Dispatcher.UIThread.Post(() =>
            {
                foreach (var dCh in _digitalChannels)
                {
                    if (savedDigitalStates.TryGetValue(dCh.Name, out var savedOn))
                        dCh.IsOn = savedOn;
                }

                foreach (var pCh in _parameterChannels)
                {
                    if (savedParamSettings.TryGetValue(pCh.Name, out var val))
                        pCh.SetValue = val;
                }
            });
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Lỗi kết nối Database: {ex.Message}";
        }

        // 2. Kết nối MQTT Broker & đồng bộ trạng thái
        try
        {
            await _mqttClient.ConnectAsync();
            ConnectionStatus = "Đã kết nối MQTT Broker & PostgreSQL";

            foreach (var dCh in _digitalChannels)
            {
                if (savedDigitalStates.TryGetValue(dCh.Name, out var savedOn))
                {
                    await _mqttClient.SendValueAsync(dCh.Topic, savedOn ? "1" : "0");
                }
            }
        }
        catch
        {
            ConnectionStatus = "Kết nối Broker thất bại!";
        }

        // 3. Subscribe toàn bộ các kênh
        foreach (var dCh in _digitalChannels)
            await _mqttClient.SubscribeTopicAsync(dCh.Topic);

        foreach (var pCh in _parameterChannels)
            await _mqttClient.SubscribeTopicAsync(pCh.ReadTopic);
    }

    public async Task SaveChannelAsync(ParameterChannelViewModel channel, string value)
    {
        channel.SetValue = value;
        await _mqttClient.SendValueAsync(channel.WriteTopic, value);
        await SaveStateAsync();
    }

    private async Task SaveStateAsync()
    {
        try
        {
            var digitalDict = _digitalChannels.ToDictionary(c => c.Name, c => c.IsOn);
            var paramDict = _parameterChannels.ToDictionary(c => c.Name, c => c.SetValue);

            await _dataLogger.SaveDigitalStatesAsync(digitalDict);
            await _dataLogger.SaveParameterSettingsAsync(paramDict);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PostgreSQL Save Error]: {ex.Message}");
        }
    }
}