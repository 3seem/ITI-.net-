// =====================================================================
// RagPipeline — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// Wires Retrieval + Augmentation + Generation into the one method
// Block 3 builds live: Ask(question). This is the whole of "RAG" in
// three steps, made concrete:
//   1. RETRIEVE  — find the K most relevant real documents
//   2. AUGMENT   — paste them into the prompt as context
//   3. GENERATE  — let the chat provider answer USING that context
// =====================================================================
namespace StudentPortalAI
{
    public class RagPipeline
    {
        private readonly Retriever _retriever;
        private readonly IChatProvider _chatProvider;
        private const int TopK = 2;

        public const string SystemPrompt =
            "You are the StudentPortal assistant. Answer only using the retrieved " +
            "StudentPortal records you are given. If they don't contain the answer, say so.";

        public RagPipeline(Retriever retriever, IChatProvider chatProvider)
        {
            _retriever = retriever;
            _chatProvider = chatProvider;
        }

        public string Ask(string question)
        {
            var retrieved = _retriever.TopK(question, TopK);

            // AUGMENT: join the retrieved documents' real text into one
            // context block. This is literally string concatenation —
            // deliberately shown as exactly that, because "prompt
            // augmentation" sounds more mysterious than it is.
            string context = string.Join(" ", retrieved.Select(r => r.Document.Text));

            return _chatProvider.Complete(SystemPrompt, context, question);
        }

        // Runs the SAME question with NO retrieval step at all — used by
        // Block 2's "ungrounded run" to demonstrate the Warm-Up
        // prediction live, and to make the before/after of Block 3
        // visible and honest (the "before" the RAG payoff is compared
        // against).
        public string AskUngrounded(string question)
        {
            return _chatProvider.Complete(SystemPrompt, groundingContext: null, question);
        }
    }
}
