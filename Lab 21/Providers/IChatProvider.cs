// =====================================================================
// IChatProvider — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// A chat provider takes a SYSTEM prompt (instructions about how to
// behave), an optional block of GROUNDING CONTEXT (retrieved documents,
// or none), and a USER prompt (the actual question), and returns an
// answer. Every real LLM API — OpenAI, Azure OpenAI, Anthropic, Google —
// shapes its request this way (system/context/user), even though the
// exact JSON field names differ (BUILD_PLAN.md §4, Block 6).
// =====================================================================
namespace StudentPortalAI
{
    public interface IChatProvider
    {
        string Complete(string systemPrompt, string? groundingContext, string userPrompt);
    }
}
