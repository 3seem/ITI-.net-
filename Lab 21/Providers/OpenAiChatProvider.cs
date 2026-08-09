// =====================================================================
// OpenAiChatProvider — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// OPTIONAL, OPT-IN ONLY (BUILD_PLAN.md §5) — same status as
// OpenAiEmbeddingProvider. Calls the real chat-completions endpoint.
// Model named generically here as a current-generation, low-cost chat
// model — BUILD_PLAN.md §4 deliberately does NOT pin an exact model
// version in a claim this session verifies, because the model lineup
// changes faster than this file will. Set the real model name from the
// environment so this file never goes stale on its own.
// =====================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentPortalAI
{
    public class OpenAiChatProvider : IChatProvider
    {
        private readonly HttpClient _http;
        private readonly string _model;

        public OpenAiChatProvider(string apiKey, string? model = null)
        {
            _http = new HttpClient { BaseAddress = new Uri("https://api.openai.com/v1/") };
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            // Reads the real model name from OPENAI_MODEL if set, so
            // whoever runs the --live demo picks whatever current
            // model their key has access to, without editing this file.
            _model = model
                ?? Environment.GetEnvironmentVariable("OPENAI_MODEL")
                ?? "gpt-5.4-mini";
        }

        public string Complete(string systemPrompt, string? groundingContext, string userPrompt)
        {
            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt }
            };

            if (!string.IsNullOrWhiteSpace(groundingContext))
            {
                messages.Add(new
                {
                    role = "system",
                    content = "Use ONLY the following retrieved facts to answer. " +
                               "If the facts don't cover the question, say so — never guess.\n\n" +
                               groundingContext
                });
            }

            messages.Add(new { role = "user", content = userPrompt });

            var response = _http.PostAsJsonAsync("chat/completions", new
            {
                model = _model,
                messages
            }).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }
    }
}
