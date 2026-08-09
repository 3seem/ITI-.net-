// =====================================================================
// SimpleAgent — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// This is Block 8's payoff pipeline. An "agent," at its simplest, is a
// program that looks at a question and DECIDES what to do next instead
// of always doing the same fixed thing. Ours checks its tools in order
// and asks each "can you handle this?" — the first one that says yes
// runs. If NONE of the precise tools can handle it, it falls back to
// the RAG pipeline built in Block 3, which searches by similarity
// instead of an exact match. That fallback chain — try a precise tool
// first, fall back to a fuzzy search — is a real, common agent pattern,
// not a simplification invented for this course.
// =====================================================================
namespace StudentPortalAI
{
    public class SimpleAgent
    {
        private readonly List<ITool> _tools;
        private readonly RagPipeline _ragFallback;

        public SimpleAgent(List<ITool> tools, RagPipeline ragFallback)
        {
            _tools = tools;
            _ragFallback = ragFallback;
        }

        public AgentResult Handle(string question)
        {
            foreach (var tool in _tools)
            {
                if (tool.CanHandle(question))
                {
                    return new AgentResult
                    {
                        ToolUsed = tool.Name,
                        Answer = tool.Execute(question)
                    };
                }
            }

            return new AgentResult
            {
                ToolUsed = "rag_fallback",
                Answer = _ragFallback.Ask(question)
            };
        }
    }

    public class AgentResult
    {
        public string ToolUsed { get; set; } = "";
        public string Answer { get; set; } = "";
    }
}
