// =====================================================================
// StudentPortalAI — COMPLETE REFERENCE SOLUTION
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — GenAI Fundamentals + AI-Assisted Coding + AI Agents/
//              Automation/Wrap-Up (THE LAST SESSION OF THE TRACK)
//
// Reads the real, live ITI_StudentPortal database (read-only — see
// Data/StudentPortalReader.cs) and demonstrates, with fully
// deterministic and offline output by default, the whole arc of today:
// an ungrounded question that a model correctly declines to guess at,
// a RAG pipeline that grounds the same question in real retrieved
// data, and a small agent that decides whether to use a precise tool
// or fall back to RAG.
//
// USAGE (all offline/mock by default — no API key, no internet needed):
//   dotnet run -- --mode ungrounded  -- Block 2's live demo
//   dotnet run -- --mode rag         -- Block 3's live demo
//   dotnet run -- --mode agent       -- Block 8's live demo (the payoff)
//   dotnet run -- --mode all         -- runs all three in sequence (default)
//
// OPTIONAL real API call (BUILD_PLAN.md §5 — opt-in only):
//   dotnet run -- --mode agent --live
//   (requires OPENAI_API_KEY to be set as an environment variable)
// =====================================================================
namespace StudentPortalAI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string mode = GetArgValue(args, "--mode") ?? "all";
            bool live = args.Contains("--live");

            Console.WriteLine("=====================================================");
            Console.WriteLine(" StudentPortalAI — Session 21 (GenAI, RAG, Agents)");
            Console.WriteLine(" Reading the REAL ITI_StudentPortal database...");
            Console.WriteLine("=====================================================");

            var reader = new StudentPortalReader();
            List<KnowledgeDocument> studentDocs;
            List<KnowledgeDocument> courseDocs;

            try
            {
                studentDocs = reader.LoadStudentDocuments();
                courseDocs = reader.LoadCourseDocuments();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Could not read ITI_StudentPortal: {ex.Message}");
                Console.WriteLine("Check Data/StudentPortalReader.cs's connection string and that");
                Console.WriteLine("SQL Server is reachable at 'Server=.' (same as StudentPortalWeb).");
                return;
            }

            Console.WriteLine($"Loaded {studentDocs.Count} student record(s) and " +
                               $"{courseDocs.Count} course record(s).");
            Console.WriteLine();

            var allDocs = new List<KnowledgeDocument>();
            allDocs.AddRange(studentDocs);
            allDocs.AddRange(courseDocs);

            IEmbeddingProvider embeddingProvider;
            IChatProvider chatProvider;

            if (live)
            {
                string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.WriteLine("[ERROR] --live was passed but OPENAI_API_KEY is not set.");
                    Console.WriteLine("Falling back to the offline mock providers instead.");
                    embeddingProvider = new MockEmbeddingProvider();
                    chatProvider = new MockChatProvider();
                }
                else
                {
                    Console.WriteLine("[LIVE MODE] Using the real OpenAI API. This costs a small,");
                    Console.WriteLine("real amount of money (BUILD_PLAN.md §4).");
                    embeddingProvider = new OpenAiEmbeddingProvider(apiKey);
                    chatProvider = new OpenAiChatProvider(apiKey);
                }
            }
            else
            {
                embeddingProvider = new MockEmbeddingProvider();
                chatProvider = new MockChatProvider();
            }

            var retriever = new Retriever(embeddingProvider, allDocs);
            var rag = new RagPipeline(retriever, chatProvider);

            // The one question used identically all day (BUILD_PLAN.md §9).
            // "Kareem Fouad" is a real, confirmed row from 00_inspect.bat's
            // results (Id 4, Year 4, GPA 3.2) — unique full name, no
            // duplicate-name collision risk (unlike "Mona Khaled"/"Sara Nabil").
            string payoffQuestion = "What honour band is Kareem Fouad in?";

            if (mode is "ungrounded" or "all")
            {
                RunUngroundedDemo(rag, payoffQuestion);
            }

            if (mode is "rag" or "all")
            {
                RunRagDemo(rag, payoffQuestion);
            }

            if (mode is "agent" or "all")
            {
                RunAgentDemo(reader, studentDocs, courseDocs, embeddingProvider, chatProvider, payoffQuestion);
            }
        }

        // ---- Block 2's live demo: the Warm-Up prediction, run for real ----
        private static void RunUngroundedDemo(RagPipeline rag, string question)
        {
            Console.WriteLine("--- UNGROUNDED (no retrieval) ---");
            Console.WriteLine($"Q: {question}");
            string answer = rag.AskUngrounded(question);
            Console.WriteLine($"A: {answer}");
            Console.WriteLine();
        }

        // ---- Block 3's live demo: the RAG mini-exercise ----
        private static void RunRagDemo(RagPipeline rag, string question)
        {
            Console.WriteLine("--- GROUNDED (RAG: retrieve, then answer) ---");
            Console.WriteLine($"Q: {question}");
            string answer = rag.Ask(question);
            Console.WriteLine($"A: {answer}");
            Console.WriteLine();
        }

        // ---- Block 8's live demo: the agent, and the whole day's payoff ----
        private static void RunAgentDemo(
            StudentPortalReader reader,
            List<KnowledgeDocument> studentDocs,
            List<KnowledgeDocument> courseDocs,
            IEmbeddingProvider embeddingProvider,
            IChatProvider chatProvider,
            string question)
        {
            var studentTool = new StudentLookupTool(reader)
            {
                KnownStudentNames = studentDocs
                    .Select(d => d.Text.Split(" is a ")[0])
                    .ToList()
            };
            var courseTool = new CourseLookupTool(courseDocs);
            var instructorTool = new InstructorLookupTool(courseDocs);

            var allDocs = new List<KnowledgeDocument>();
            allDocs.AddRange(studentDocs);
            allDocs.AddRange(courseDocs);
            var retriever = new Retriever(embeddingProvider, allDocs);
            var ragFallback = new RagPipeline(retriever, chatProvider);

            var agent = new SimpleAgent(new List<ITool> { studentTool, courseTool, instructorTool }, ragFallback);

            Console.WriteLine("--- AGENT (picks a tool, or falls back to RAG) ---");
            Console.WriteLine($"Q: {question}");
            var result = agent.Handle(question);
            Console.WriteLine($"tool used: {result.ToolUsed}]");
            Console.WriteLine($"A: {result.Answer}");
            Console.WriteLine();

            Console.WriteLine("This is the day's payoff: the same question the Warm-Up predicted");
            Console.WriteLine("on paper, now answered correctly — because the agent was given the");
            Console.WriteLine("one thing a bare model never had: your own real data.");


            string instructorQuestion = "Who teaches Web Development Using .NET?"; 
            Console.WriteLine(" AGENT instructor_lookup");
            Console.WriteLine($"Q: {instructorQuestion}");
            var instructorResult = agent.Handle(instructorQuestion);
            Console.WriteLine($"tool used: {instructorResult.ToolUsed}");
            Console.WriteLine($"A: {instructorResult.Answer}");
        }

        private static string? GetArgValue(string[] args, string flag)
        {
            int index = Array.IndexOf(args, flag);
            if (index >= 0 && index + 1 < args.Length)
            {
                return args[index + 1];
            }
            return null;
        }
    }
}
