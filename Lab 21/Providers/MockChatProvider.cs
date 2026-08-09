// =====================================================================
// MockChatProvider — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// The offline, deterministic, zero-cost, zero-network default chat
// provider — the TAUGHT and VERIFIED path (BUILD_PLAN.md §5). Every
// string this class returns is exact and repeatable, which is what
// makes this session's expected-output blocks real, checkable text
// instead of "run it and see what the model says this time."
//
// 📌 Students May Ask — "Real LLMs don't just paste text into a
// template, do they?"
// Correct, and this is disclosed on purpose (Rule 17). A real model can
// paraphrase, combine several documents, and handle a question phrased
// ten different ways. What it CANNOT do — no matter how good the model
// is — is correctly answer a question about data it was never shown.
// That is the ONE property this mock proves for real, because its
// "grounded" answer is built directly from the actual retrieved row.
// That is the property RAG exists to fix, and it is the property this
// whole session's payoff (BUILD_PLAN.md §6) demonstrates live.
// =====================================================================
namespace StudentPortalAI
{
    public class MockChatProvider : IChatProvider
    {
        public string Complete(string systemPrompt, string? groundingContext, string userPrompt)
        {
            // UNGROUNDED MODE — no retrieved context was passed in.
            // This is the CORRECT, honest behavior of a model with no
            // access to private data: decline rather than invent an
            // answer. A model that guesses anyway is the more dangerous
            // failure mode, and the guide names both explicitly.
            if (string.IsNullOrWhiteSpace(groundingContext))
            {
                return "I don't have access to any private student records, so I can't answer " +
                       "that accurately. I could guess, but I'd rather tell you I don't know than " +
                       "make something up.";
            }

            // GROUNDED MODE — real retrieved rows were passed in as
            // context. The answer is built directly from that real
            // text, so it is correct because it is reading real data —
            // not because the model reasoned its way to the right
            // answer.
            return $"Based on the StudentPortal records I was given: {groundingContext}";
        }
    }
}
