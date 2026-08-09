// =====================================================================
// KnowledgeDocument — StudentPortalAI
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// One "document" in our tiny knowledge base. In a production RAG system
// this would usually be a chunk of a longer document (a paragraph of a
// PDF, a support ticket, a wiki page). Here, each document is one real
// row from the real ITI_StudentPortal database, turned into a plain
// sentence an embedding model (real or mock) can compare against a
// question.
// =====================================================================
namespace StudentPortalAI
{
    public class KnowledgeDocument
    {
        // Where this fact came from — kept so the RAG pipeline can show
        // its work ("this answer came from row Students#7").
        public string SourceTable { get; set; } = "";
        public int SourceId { get; set; }

        // The plain-English sentence version of the real row. This is
        // what gets embedded and what gets shown to the chat provider
        // as grounding context.
        public string Text { get; set; } = "";
    }

    // A document plus how well it matched a specific question. Only
    // produced by the Retriever, never constructed directly from the DB.
    public class RetrievedDocument
    {
        public KnowledgeDocument Document { get; set; } = null!;
        public double Score { get; set; }
    }
}
