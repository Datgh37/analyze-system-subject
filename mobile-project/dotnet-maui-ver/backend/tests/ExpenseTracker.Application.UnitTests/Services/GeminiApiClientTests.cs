using System.Net;
using System.Text;
using ExpenseTracker.Application.Common.Settings;
using ExpenseTracker.Infrastructure.Ai;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class GeminiApiClientTests
{
    private readonly Mock<ILogger<GeminiApiClient>> _loggerMock;

    public GeminiApiClientTests()
    {
        _loggerMock = new Mock<ILogger<GeminiApiClient>>();
    }

    [Fact]
    public async Task GenerateContentWithImageAsync_WhenApiKeyIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var settings = Options.Create(new AiSettings { ApiKey = "" });
        var client = new GeminiApiClient(new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, "")), settings, _loggerMock.Object);

        // Act
        var result = await client.GenerateContentWithImageAsync(new byte[] { 1, 2, 3 }, "image/jpeg", "Extract invoice");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("chưa được cấu hình");
    }

    [Fact]
    public async Task GenerateContentWithImageAsync_WhenImageIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var settings = Options.Create(new AiSettings { ApiKey = "mock_key" });
        var client = new GeminiApiClient(new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, "")), settings, _loggerMock.Object);

        // Act
        var result = await client.GenerateContentWithImageAsync(Array.Empty<byte>(), "image/jpeg", "Extract invoice");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("ảnh không hợp lệ");
    }

    [Fact]
    public async Task GenerateContentWithImageAsync_WhenGeminiReturns200WithValidJson_ShouldReturnSuccess()
    {
        // Arrange
        var geminiResponseJson = """
        {
          "candidates": [
            {
              "content": {
                "parts": [
                  {
                    "text": "{\"merchantName\": \"WinMart\", \"totalAmount\": 150000}"
                  }
                ]
              }
            }
          ]
        }
        """;

        var settings = Options.Create(new AiSettings { ApiKey = "mock_valid_key", EnableLogging = false });
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, geminiResponseJson);
        var httpClient = new HttpClient(handler);
        var client = new GeminiApiClient(httpClient, settings, _loggerMock.Object);

        // Act
        var result = await client.GenerateContentWithImageAsync(new byte[] { 1, 2, 3, 4 }, "image/jpeg", "Extract invoice");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain("WinMart");
        result.Value.Should().Contain("150000");
    }

    [Fact]
    public async Task GenerateContentWithImageAsync_WhenGeminiReturns401_ShouldReturnFailureWithKeyError()
    {
        // Arrange
        var errorJson = "{\"error\": {\"code\": 401, \"message\": \"API key not valid.\"}}";
        var settings = Options.Create(new AiSettings { ApiKey = "invalid_key", EnableLogging = false });
        var handler = new MockHttpMessageHandler(HttpStatusCode.Unauthorized, errorJson);
        var httpClient = new HttpClient(handler);
        var client = new GeminiApiClient(httpClient, settings, _loggerMock.Object);

        // Act
        var result = await client.GenerateContentWithImageAsync(new byte[] { 1, 2, 3, 4 }, "image/jpeg", "Extract invoice");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("401");
    }

    [Fact]
    public async Task GenerateContentWithImageAsync_WhenGeminiReturns429_ShouldReturnFailureWithQuotaError()
    {
        // Arrange
        var errorJson = "{\"error\": {\"code\": 429, \"message\": \"Resource has been exhausted.\"}}";
        var settings = Options.Create(new AiSettings { ApiKey = "rate_limited_key", EnableLogging = false });
        var handler = new MockHttpMessageHandler(HttpStatusCode.TooManyRequests, errorJson);
        var httpClient = new HttpClient(handler);
        var client = new GeminiApiClient(httpClient, settings, _loggerMock.Object);

        // Act
        var result = await client.GenerateContentWithImageAsync(new byte[] { 1, 2, 3, 4 }, "image/jpeg", "Extract invoice");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Quota Exceeded");
    }

    [Fact]
    public async Task GenerateTextAsync_WhenGeminiReturns200_ShouldReturnExtractedText()
    {
        // Arrange
        var geminiResponseJson = """
        {
          "candidates": [
            {
              "content": {
                "parts": [
                  {
                    "text": "{\"intent\": \"AddExpense\", \"amount\": 50000}"
                  }
                ]
              }
            }
          ]
        }
        """;

        var settings = Options.Create(new AiSettings { ApiKey = "mock_valid_key", EnableLogging = false });
        var handler = new MockHttpMessageHandler(HttpStatusCode.OK, geminiResponseJson);
        var httpClient = new HttpClient(handler);
        var client = new GeminiApiClient(httpClient, settings, _loggerMock.Object);

        // Act
        var result = await client.GenerateTextAsync("Cà phê sáng 50k");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain("AddExpense");
        result.Value.Should().Contain("50000");
    }

    [Fact]
    public async Task GenerateTextAsync_WhenPromptIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var settings = Options.Create(new AiSettings { ApiKey = "mock_key" });
        var client = new GeminiApiClient(new HttpClient(new MockHttpMessageHandler(HttpStatusCode.OK, "")), settings, _loggerMock.Object);

        // Act
        var result = await client.GenerateTextAsync("");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("không được để trống");
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;

        public MockHttpMessageHandler(HttpStatusCode statusCode, string responseContent)
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}
