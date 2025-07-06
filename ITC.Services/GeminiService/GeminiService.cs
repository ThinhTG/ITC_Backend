using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ITC.Services.GeminiService
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string[] _availableModels = {
            "gemini-1.5-flash",
            "gemini-1.5-pro", 
            "gemini-1.0-pro"
        };

        public GeminiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["GeminiAI:ApiKey"] ?? throw new ArgumentNullException("GeminiAI:ApiKey configuration is missing");
        }

        public async Task<string> GetSuggestionAsync(string prompt)
        {
            Exception? lastException = null;

            foreach (var model in _availableModels)
            {
                try
                {
                    var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={_apiKey}";

                    var body = new
                    {
                        contents = new[]
                        {
                            new {
                                parts = new[]
                                {
                                    new { text = prompt }
                                }
                            }
                        }
                    };

                    var json = JsonSerializer.Serialize(body);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(url, content);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        lastException = new HttpRequestException($"Gemini API request failed for model {model}: {response.StatusCode} - {errorContent}");
                        continue; // Try next model
                    }

                    var responseString = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(responseString);
                    return doc.RootElement
                              .GetProperty("candidates")[0]
                              .GetProperty("content")
                              .GetProperty("parts")[0]
                              .GetProperty("text")
                              .GetString() ?? "No response from Gemini";
                }
                catch (Exception ex)
                {
                    lastException = new Exception($"Error calling Gemini API with model {model}: {ex.Message}", ex);
                    continue; // Try next model
                }
            }

            // If all models failed
            throw lastException ?? new Exception("All Gemini models failed");
        }
    }
} 