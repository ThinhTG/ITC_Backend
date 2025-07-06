namespace ITC.Services.GeminiService
{
    public interface IGeminiService
    {
        Task<string> GetSuggestionAsync(string prompt);
    }
} 