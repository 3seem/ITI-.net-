// =====================================================================
// OpenAiEmbeddingProvider — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// OPTIONAL, OPT-IN ONLY (BUILD_PLAN.md §5). Nothing taught, graded, or
// verified in this session depends on this class working. It exists so
// that IF Hamdy has a funded OPENAI_API_KEY and wants to show one real
// round trip live, the wiring is already correct and needs zero new
// code written in front of the room.
//
// Calls the real OpenAI-compatible embeddings endpoint over HTTPS.
// Model: text-embedding-3-small — priced at $0.02 per 1M tokens
// (standard tier), current per BUILD_PLAN.md §4's Aug 8 2026 check.
// A single classroom demo call costs a small fraction of a cent.
// =====================================================================
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace StudentPortalAI
{
    public class OpenAiEmbeddingProvider : IEmbeddingProvider
    {
        private readonly HttpClient _http;
        private const string Model = "text-embedding-3-small";

        public OpenAiEmbeddingProvider(string apiKey)
        {
            _http = new HttpClient { BaseAddress = new Uri("https://api.openai.com/v1/") };
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public float[] Embed(string text)
        {
            // Synchronous wrapper — deliberate, for this small teaching
            // console app. A production service would be async
            // end-to-end (Session 19 already taught why: async, LINQ
            // Part 2/EF Core). Disclosed, not hidden.
            var response = _http.PostAsJsonAsync("embeddings", new
            {
                model = Model,
                input = text
            }).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            using var doc = JsonDocument.Parse(json);

            var embeddingArray = doc.RootElement
                .GetProperty("data")[0]
                .GetProperty("embedding");

            var vector = new float[embeddingArray.GetArrayLength()];
            int i = 0;
            foreach (var value in embeddingArray.EnumerateArray())
            {
                vector[i++] = value.GetSingle();
            }

            return vector;
        }
    }
}
