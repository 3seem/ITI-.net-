// =====================================================================
// IEmbeddingProvider — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// An embedding turns a piece of text into a list of numbers (a
// "vector") such that texts with similar MEANING end up as similar
// numbers. This is the contract both a real embedding model and our
// offline teaching stand-in implement identically, so the rest of the
// pipeline (Retriever) never needs to know or care which one is behind
// the interface. That swap — mock today, real provider later, zero
// other files touched — IS the point of coding to an interface, and
// this track has taught that lesson before (Session 26 elsewhere on
// this curriculum calls it out explicitly with payment providers).
// =====================================================================
namespace StudentPortalAI
{
    public interface IEmbeddingProvider
    {
        float[] Embed(string text);
    }
}
