using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using prototype_mqtt.Models;

namespace prototype_mqtt.Services;
public class JsonDataLogger
{
    private readonly string _filePath;

    public JsonDataLogger(string fileName = "app_state.json")
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }

    // Save the complete state (LED + SetValue + CurrentValue)
    public async Task SaveStateAsync(AppStateData data)
    {
        try
        {
            data.LastUpdated = DateTime.Now;
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(_filePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Logger Error] Không thể lưu JSON: {ex.Message}");
        }
    }

    
    public async Task<AppStateData?> LoadStateAsync()
    {
        try
        {
            if (!File.Exists(_filePath)) return null;

            string jsonString = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<AppStateData>(jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Logger Error] Không thể đọc JSON: {ex.Message}");
            return null;
        }
    }
}