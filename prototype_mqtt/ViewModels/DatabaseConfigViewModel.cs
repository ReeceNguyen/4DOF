using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Npgsql;
using prototype_mqtt.Models;

namespace prototype_mqtt.ViewModels;

public partial class DatabaseConfigViewModel : ViewModelBase
{
    private const string ConfigFileName = "db_config.json";

    [ObservableProperty] private string _host = "127.0.0.1";
    [ObservableProperty] private int _port = 5432;
    [ObservableProperty] private string _username = "opta";
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _database = "prototype_mqtt";
    [ObservableProperty] private string _statusMessage = string.Empty;

    public event Action<DatabaseConfig>? OnLoginSuccess;

    public DatabaseConfigViewModel()
    {
        var current = LoadConfig();
        Host = current.Host;
        Port = current.Port;
        Username = current.Username;
        Password = current.Password;
        Database = current.Database;
    }

    public static DatabaseConfig LoadConfig()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
        if (!File.Exists(path)) return new DatabaseConfig();

        try
        {
            return JsonSerializer.Deserialize<DatabaseConfig>(File.ReadAllText(path)) ?? new DatabaseConfig();
        }
        catch
        {
            return new DatabaseConfig();
        }
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        StatusMessage = "Đang xác thực kết nối PostgreSQL...";
        var config = new DatabaseConfig
        {
            Host = Host.Trim(),
            Port = Port,
            Username = Username.Trim(),
            Password = Password,
            Database = Database.Trim()
        };

        try
        {
            // Kiểm tra kết nối trực tiếp tới PostgreSQL
            await using var conn = new NpgsqlConnection(config.GetConnectionString());
            await conn.OpenAsync();

            // Lưu lại cấu hình cho lần đăng nhập sau
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
            File.WriteAllText(path, JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true }));

            StatusMessage = string.Empty;
            OnLoginSuccess?.Invoke(config);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Đăng nhập thất bại: {ex.Message}";
        }
    }
}