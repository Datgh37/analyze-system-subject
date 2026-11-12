namespace ExpenseTracker.Application.Common.Settings;

// Configuration settings for AI Foundation Models (e.g., Google Gemini 2.5 Flash)
public class AiSettings
{
    public const string SectionName = "AiSettings";

    public string Provider { get; set; } = "Gemini";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
    public string Endpoint { get; set; } = "https://generativelanguage.googleapis.com/v1beta";
    public double Temperature { get; set; } = 0.2;
    public int MaxTokens { get; set; } = 4096;
    public bool EnableLogging { get; set; } = true;
}
