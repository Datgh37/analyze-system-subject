using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Enterprise AI Gateway interface communicating directly with Google Gemini 2.5 Flash API
public interface IGeminiApiClient
{
    // Sends multimodal image payload (Base64) to Google Gemini 2.5 Flash with strict JSON Schema output
    Task<Result<string>> GenerateContentWithImageAsync(
        byte[] imageBytes,
        string mimeType,
        string prompt,
        string? systemInstruction = null,
        CancellationToken ct = default);

    // Sends text prompt to Google Gemini 2.5 Flash for NLP reasoning, function calling, or empathetic financial commentary
    Task<Result<string>> GenerateTextAsync(
        string prompt,
        string? systemInstruction = null,
        CancellationToken ct = default);
}
