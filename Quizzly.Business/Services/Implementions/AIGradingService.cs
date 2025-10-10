using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.AI;
using System.Text;

public class AIGradingService : IAIGradingService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AIGradingService(IConfiguration configuration)
    {
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task<GradingResponse> AiGradeAnswerAsync(string questionText, string studentAnswer, string modelAnswer, int maxPoint)
    {
        var apiKey = _configuration["Ai:ApiKey"];
        var baseUrl = _configuration["Ai:BaseUrl"] ?? "https://api.groq.com/openai/v1";
        var model = _configuration["Ai:Model"] ?? "llama-3.1-70b-versatile";

        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("AI API key is not configured.");

        // Construct the grading prompt
        var prompt =
            $"You are an expert grader. Grade the student's answer fairly based on the model answer.\n\n" +
            $"---\n" +
            $"Question:\n{questionText}\n\n" +
            $"Model Answer:\n{modelAnswer}\n\n" +
            $"Student Answer:\n{studentAnswer}\n" +
            $"---\n\n" +
            $"Instructions:\n" +
            $"- Score between 0 and {maxPoint} (0 = completely incorrect, {maxPoint} = perfect answer).\n" +
            $"- Focus on how accurate and complete the student's answer is compared to the model.\n" +
            $"- Give short, encouraging feedback that helps the student improve — no long explanations.\n\n" +
            $"Respond only in valid JSON format with these two fields:\n" +
            $"{{\n  \"score\": <numeric_value>,\n  \"feedback\": \"<one short sentence of feedback for the student>\"\n}}";


        // Prepare the API request payload
        var requestBody = new
        {
            model = model,
            messages = new[]
            {
                new { role = "system", content = "You are an expert academic grader. Always respond in valid JSON format." },
                new { role = "user", content = prompt }
            },
            temperature = 0.3, // Lower temperature for more consistent grading
            max_tokens = 500,
            response_format = new { type = "json_object" } // Force JSON response
        };

        var jsonContent = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Set authorization header
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        try
        {
            // Make API request
            var response = await _httpClient.PostAsync($"{baseUrl}/chat/completions", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"AI API request failed with status {response.StatusCode}: {responseContent}");
            }

            // Parse the response
            var jsonResponse = JObject.Parse(responseContent);
            var aiMessage = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();

            if (string.IsNullOrEmpty(aiMessage))
            {
                throw new InvalidOperationException("AI API returned empty response.");
            }

            // Parse the AI's JSON response
            var gradingResult = JObject.Parse(aiMessage);
            var score = gradingResult["score"]?.Value<int>() ?? 0;
            var feedback = gradingResult["feedback"]?.Value<string>() ?? "No feedback provided.";

            // Validate score is within bounds
            if (score < 0) score = 0;
            if (score > maxPoint) score = maxPoint;

            return new GradingResponse
            {
                Score = score,
                Feedback = feedback
            };
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse AI response as JSON: {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Network error calling AI API: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unexpected error during AI grading: {ex.Message}");
        }
    }
}