// =====================================================================
// ITool — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// A "tool" is anything an agent can choose to call instead of, or in
// addition to, just generating text — a database lookup, a web search,
// a calculator, an email sender. Real agent frameworks (LangChain,
// Microsoft Agent Framework) let the MODEL ITSELF decide which tool to
// call, using a technique called function calling — the model is shown
// a list of tools and their parameters, and it outputs a structured
// request to call one, that YOUR code then actually executes.
//
// 📌 Students May Ask — "So does SimpleAgent use real function
// calling?"
// No, and that's disclosed on purpose (Rule 17). SimpleAgent below
// picks a tool with a simple keyword rule so the whole exercise stays
// offline, deterministic, and verifiable (BUILD_PLAN.md §5). A real
// agent asks the MODEL to choose, using exactly this same shape —
// name, description, parameters — sent to the model as JSON. Block 8
// shows that real JSON shape as a labeled example, not something this
// mock actually sends anywhere.
// =====================================================================
namespace StudentPortalAI
{
    public interface ITool
    {
        string Name { get; }
        string Description { get; }

        // Returns true if this tool can handle the question at all.
        bool CanHandle(string question);

        string Execute(string question);
    }
}
