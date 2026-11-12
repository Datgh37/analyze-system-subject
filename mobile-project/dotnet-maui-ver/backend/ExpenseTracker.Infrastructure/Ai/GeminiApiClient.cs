using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ExpenseTracker.Application.Common.Settings;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExpenseTracker.Infrastructure.Ai;

// High-performance Enterprise AI Gateway connecting directly to Google Gemini 2.5 Flash REST API
public class GeminiApiClient : IGeminiApiClient
{
    private readonly HttpClient _httpClient;
    private readonly AiSettings _settings;
    private readonly ILogger<GeminiApiClient> _logger;

    public GeminiApiClient(
        HttpClient httpClient,
        IOptions<AiSettings> settings,
        ILogger<GeminiApiClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<Result<string>> GenerateContentWithImageAsync(
        byte[] imageBytes,
        string mimeType,
        string prompt,
        string? systemInstruction = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            var msg = "Gemini API Key chưa được cấu hình. Vui lòng thêm ApiKey vào appsettings.Local.json hoặc biến môi trường.";
            _logger.LogError("[Gemini] {Message}", msg);
            return Result<string>.Failure(msg);
        }

        if (imageBytes == null || imageBytes.Length == 0)
        {
            return Result<string>.Failure("Dữ liệu ảnh không hợp lệ hoặc rỗng.");
        }

        try
        {
            var base64Data = Convert.ToBase64String(imageBytes);
            var normalizedMime = string.IsNullOrWhiteSpace(mimeType) ? "image/jpeg" : mimeType;

            var requestObj = new JsonObject
            {
                ["contents"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["parts"] = new JsonArray
                        {
                            new JsonObject { ["text"] = prompt },
                            new JsonObject
                            {
                                ["inlineData"] = new JsonObject
                                {
                                    ["mimeType"] = normalizedMime,
                                    ["data"] = base64Data
                                }
                            }
                        }
                    }
                },
                ["generationConfig"] = new JsonObject
                {
                    ["temperature"] = _settings.Temperature,
                    ["maxOutputTokens"] = _settings.MaxTokens,
                    ["responseMimeType"] = "application/json"
                }
            };

            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                requestObj["systemInstruction"] = new JsonObject
                {
                    ["parts"] = new JsonArray
                    {
                        new JsonObject { ["text"] = systemInstruction }
                    }
                };
            }

            return await ExecuteGeminiRequestAsync(requestObj, imageSizeKb: imageBytes.Length / 1024.0, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Gemini] Ngoại lệ khi chuẩn bị payload gửi Gemini Multimodal Vision");
            return Result<string>.Failure($"Lỗi xử lý gửi ảnh Gemini AI: {ex.Message}");
        }
    }

    public async Task<Result<string>> GenerateTextAsync(
        string prompt,
        string? systemInstruction = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            var msg = "Gemini API Key chưa được cấu hình. Vui lòng thêm ApiKey vào appsettings.Local.json hoặc biến môi trường.";
            _logger.LogError("[Gemini] {Message}", msg);
            return Result<string>.Failure(msg);
        }

        if (string.IsNullOrWhiteSpace(prompt))
        {
            return Result<string>.Failure("Nội dung prompt không được để trống.");
        }

        try
        {
            var requestObj = new JsonObject
            {
                ["contents"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["parts"] = new JsonArray
                        {
                            new JsonObject { ["text"] = prompt }
                        }
                    }
                },
                ["generationConfig"] = new JsonObject
                {
                    ["temperature"] = _settings.Temperature,
                    ["maxOutputTokens"] = _settings.MaxTokens,
                    ["responseMimeType"] = "application/json"
                }
            };

            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                requestObj["systemInstruction"] = new JsonObject
                {
                    ["parts"] = new JsonArray
                    {
                        new JsonObject { ["text"] = systemInstruction }
                    }
                };
            }

            return await ExecuteGeminiRequestAsync(requestObj, imageSizeKb: 0, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Gemini] Ngoại lệ khi chuẩn bị payload gửi Gemini Text NLP");
            return Result<string>.Failure($"Lỗi xử lý gửi văn bản Gemini AI: {ex.Message}");
        }
    }

    private async Task<Result<string>> ExecuteGeminiRequestAsync(
        JsonObject requestPayload,
        double imageSizeKb,
        CancellationToken ct)
    {
        var endpoint = _settings.Endpoint.TrimEnd('/');
        var candidateModels = new List<string>();
        if (!string.IsNullOrWhiteSpace(_settings.Model))
        {
            candidateModels.Add(_settings.Model.Trim());
        }

        var defaultFallbacks = new[] { "gemini-1.5-flash", "gemini-2.0-flash", "gemini-1.5-pro" };
        foreach (var fb in defaultFallbacks)
        {
            if (!candidateModels.Contains(fb, StringComparer.OrdinalIgnoreCase))
            {
                candidateModels.Add(fb);
            }
        }

        string lastResponseBody = string.Empty;
        HttpStatusCode lastStatusCode = HttpStatusCode.OK;

        for (int i = 0; i < candidateModels.Count; i++)
        {
            var currentModel = candidateModels[i];
            var url = $"{endpoint}/models/{currentModel}:generateContent?key={_settings.ApiKey}";

            var jsonContent = new StringContent(
                requestPayload.ToJsonString(),
                Encoding.UTF8,
                "application/json");

            var sw = Stopwatch.StartNew();

            if (_settings.EnableLogging)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                var payloadDesc = imageSizeKb > 0 ? $"Image {imageSizeKb:F1} KB" : "Text Prompt";
                Console.WriteLine($"\n================================================================================");
                Console.WriteLine($"[Gemini >> Request] Model: {currentModel} | Type: {payloadDesc} | Time: {DateTime.Now:HH:mm:ss}");
                Console.ResetColor();
            }

            try
            {
                var response = await _httpClient.PostAsync(url, jsonContent, ct);
                sw.Stop();

                var responseBody = await response.Content.ReadAsStringAsync(ct);
                lastResponseBody = responseBody;
                lastStatusCode = response.StatusCode;

                // Nếu gặp lỗi 404 Model Not Found hoặc 400 Unsupported, tự động fallback sang model tiếp theo
                if ((response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest) && i + 1 < candidateModels.Count)
                {
                    var nextModel = candidateModels[i + 1];
                    if (_settings.EnableLogging)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"[Gemini >> Auto-Fallback] Model '{currentModel}' không khả dụng (HTTP {(int)response.StatusCode}). Đang tự động chuyển sang '{nextModel}'...");
                        Console.ResetColor();
                    }
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                {
                    if (_settings.EnableLogging)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Gemini << Error] HTTP {(int)response.StatusCode} {response.StatusCode} | Latency: {sw.ElapsedMilliseconds} ms");
                        Console.WriteLine($"[Gemini << Error Body]: {responseBody}");
                        Console.WriteLine($"================================================================================\n");
                        Console.ResetColor();
                    }

                    return response.StatusCode switch
                    {
                        HttpStatusCode.Unauthorized => Result<string>.Failure("Khóa Gemini API Key không hợp lệ hoặc đã bị vô hiệu hóa (HTTP 401)."),
                        HttpStatusCode.Forbidden => Result<string>.Failure("Quyền truy cập API bị từ chối hoặc sai cấu hình dự án Google Cloud (HTTP 403)."),
                        HttpStatusCode.TooManyRequests => Result<string>.Failure("Đã vượt quá hạn mức gọi Google Gemini API (Quota Exceeded HTTP 429). Vui lòng thử lại sau."),
                        _ => Result<string>.Failure($"Google Gemini API trả về mã lỗi {(int)response.StatusCode}: {responseBody}")
                    };
                }

                // Parse response from Google Gemini
                var root = JsonNode.Parse(responseBody);
                string? textPart = null;

                var candidates = root?["candidates"]?.AsArray();
                if (candidates != null && candidates.Count > 0)
                {
                    var contentParts = candidates[0]?["content"]?["parts"]?.AsArray();
                    if (contentParts != null && contentParts.Count > 0)
                    {
                        textPart = contentParts[0]?["text"]?.GetValue<string>();
                    }
                }

                if (string.IsNullOrWhiteSpace(textPart))
                {
                    _logger.LogWarning("[Gemini] API trả về HTTP 200 nhưng không tìm thấy trường text trong candidates.");
                    return Result<string>.Failure("Mô hình AI phản hồi rỗng hoặc không có kết quả hợp lệ.");
                }

                if (_settings.EnableLogging)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[Gemini << Response] Status: 200 OK | Latency: {sw.ElapsedMilliseconds} ms");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[Gemini << Raw JSON Output]:");
                    Console.WriteLine(textPart);
                    Console.WriteLine($"================================================================================\n");
                    Console.ResetColor();
                }

                return Result<string>.Success(textPart);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                sw.Stop();
                _logger.LogWarning("[Gemini] Yêu cầu gọi Gemini API bị hủy bởi client sau {Elapsed} ms.", sw.ElapsedMilliseconds);
                return Result<string>.Failure($"Yêu cầu xử lý AI bị quá hạn hoặc bị hủy từ thiết bị (Hết thời gian chờ sau {sw.ElapsedMilliseconds}ms). Vui lòng thử lại.");
            }
            catch (TaskCanceledException)
            {
                sw.Stop();
                _logger.LogWarning("[Gemini] Request bị timeout nội bộ sau {Elapsed} ms với model {Model}.", sw.ElapsedMilliseconds, currentModel);
                if (i + 1 < candidateModels.Count)
                {
                    continue;
                }
                return Result<string>.Failure($"Kết nối tới Google Gemini API bị quá hạn thời gian (Timeout sau {sw.ElapsedMilliseconds}ms). Vui lòng thử lại.");
            }
            catch (HttpRequestException ex)
            {
                sw.Stop();
                _logger.LogError(ex, "[Gemini] Lỗi kết nối mạng khi gọi Gemini API với model {Model}", currentModel);
                if (i + 1 < candidateModels.Count)
                {
                    continue;
                }
                return Result<string>.Failure($"Không thể kết nối đến máy chủ Google Gemini: {ex.Message}");
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "[Gemini] Lỗi không xác định khi gọi Gemini API với model {Model}", currentModel);
                if (i + 1 < candidateModels.Count)
                {
                    continue;
                }
                return Result<string>.Failure($"Lỗi giao tiếp Gemini AI: {ex.Message}");
            }
        }

        return Result<string>.Failure($"Google Gemini API trả về mã lỗi {(int)lastStatusCode}: {lastResponseBody}");
    }
}
