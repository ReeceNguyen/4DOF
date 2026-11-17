using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;

namespace prototype_mqtt.Services;

public class PostgresDataLogger
{
    private readonly string _connectionString;

    public PostgresDataLogger(string connectionString)
    {
        _connectionString = connectionString;
    }

    // 1. Lưu trạng thái ON/OFF của từng thiết bị Digital (UPSERT)
    public async Task SaveDigitalStatesAsync(Dictionary<string, bool> digitalStates)
    {
        const string sql = @"
            INSERT INTO digital_states (device_name, is_on, updated_at)
            VALUES (@name, @isOn, CURRENT_TIMESTAMP)
            ON CONFLICT (device_name) 
            DO UPDATE SET is_on = EXCLUDED.is_on, updated_at = CURRENT_TIMESTAMP;";

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        foreach (var kvp in digitalStates)
        {
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", kvp.Key);
            cmd.Parameters.AddWithValue("isOn", kvp.Value);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    // 2. Lưu giá trị cài đặt (Setpoint) của từng cảm biến (UPSERT)
    public async Task SaveParameterSettingsAsync(Dictionary<string, string> parameterSetValues)
    {
        const string sql = @"
            INSERT INTO parameter_settings (param_name, set_value, updated_at)
            VALUES (@name, @val, CURRENT_TIMESTAMP)
            ON CONFLICT (param_name) 
            DO UPDATE SET set_value = EXCLUDED.set_value, updated_at = CURRENT_TIMESTAMP;";

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        foreach (var kvp in parameterSetValues)
        {
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", kvp.Key);
            cmd.Parameters.AddWithValue("val", kvp.Value ?? "0");
            await cmd.ExecuteNonQueryAsync();
        }
    }

    // 3. Ghi nhận lịch sử số liệu thực tế cảm biến gửi về từ MQTT (INSERT)
    public async Task LogSensorReadingAsync(string paramName, string currentValue)
    {
        const string sql = @"
            INSERT INTO sensor_readings (param_name, current_value, recorded_at)
            VALUES (@name, @val, CURRENT_TIMESTAMP);";

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("name", paramName);
        cmd.Parameters.AddWithValue("val", currentValue);
        await cmd.ExecuteNonQueryAsync();
    }

    // 4. Đọc toàn bộ trạng thái LED & Cài đặt cảm biến gần nhất khi mở lại App
    public async Task<(Dictionary<string, bool> DigitalStates, Dictionary<string, string> ParameterSetValues)> LoadStateAsync()
    {
        var digitalStates = new Dictionary<string, bool>();
        var parameterSetValues = new Dictionary<string, string>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Đọc bảng digital_states
        await using (var cmdDigital = new NpgsqlCommand("SELECT device_name, is_on FROM digital_states;", conn))
        await using (var reader = await cmdDigital.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                digitalStates[reader.GetString(0)] = reader.GetBoolean(1);
            }
        }

        // Đọc bảng parameter_settings
        await using (var cmdParam = new NpgsqlCommand("SELECT param_name, set_value FROM parameter_settings;", conn))
        await using (var reader = await cmdParam.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                parameterSetValues[reader.GetString(0)] = reader.GetString(1);
            }
        }

        return (digitalStates, parameterSetValues);
    }
}