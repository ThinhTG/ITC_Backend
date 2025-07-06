namespace ITC.Core.Contracts
{
    public class GeminiResponse
    {
        public string Result { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }
} 