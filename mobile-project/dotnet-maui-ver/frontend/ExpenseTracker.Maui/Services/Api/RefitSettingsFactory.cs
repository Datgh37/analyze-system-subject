using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;

namespace ExpenseTracker.Maui.Services.Api;

public static class RefitSettingsFactory
{
    private static RefitSettings? _instance;

    public static RefitSettings GetSettings()
    {
        if (_instance != null) return _instance;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new FlexibleIntConverter());
        options.Converters.Add(new FlexibleNullableIntConverter());

        _instance = new RefitSettings(new SystemTextJsonContentSerializer(options));
        return _instance;
    }
}

public class FlexibleIntConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetInt32();

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (int.TryParse(str, out var val)) return val;
            if (string.Equals(str, "Income", StringComparison.OrdinalIgnoreCase)) return 1;
            if (string.Equals(str, "Expense", StringComparison.OrdinalIgnoreCase)) return 2;
            if (string.Equals(str, "Transfer", StringComparison.OrdinalIgnoreCase)) return 3;
            if (string.Equals(str, "SavingsContribution", StringComparison.OrdinalIgnoreCase)) return 4;
            if (string.Equals(str, "SavingsWithdrawal", StringComparison.OrdinalIgnoreCase)) return 5;
        }

        return 0;
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

public class FlexibleNullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;
        if (reader.TokenType == JsonTokenType.Number) return reader.GetInt32();

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str)) return null;
            if (int.TryParse(str, out var val)) return val;

            if (string.Equals(str, "Needs", StringComparison.OrdinalIgnoreCase)) return 1;
            if (string.Equals(str, "Wants", StringComparison.OrdinalIgnoreCase)) return 2;
            if (string.Equals(str, "SavingsInvestments", StringComparison.OrdinalIgnoreCase)) return 3;
            if (string.Equals(str, "IncomeSource", StringComparison.OrdinalIgnoreCase)) return 4;
            if (string.Equals(str, "Income", StringComparison.OrdinalIgnoreCase)) return 1;
            if (string.Equals(str, "Expense", StringComparison.OrdinalIgnoreCase)) return 2;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue) writer.WriteNumberValue(value.Value);
        else writer.WriteNullValue();
    }
}
