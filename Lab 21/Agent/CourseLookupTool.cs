// =====================================================================
// CourseLookupTool — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// The second precise tool. Same shape as StudentLookupTool by design —
// once a trainee understands one ITool, the second is recognition, not
// new learning. This is exactly what the Lab (Rule 37) asks each
// trainee to do a THIRD time, with their own new intent.
// =====================================================================
namespace StudentPortalAI
{
    public class CourseLookupTool : ITool
    {
        private readonly List<KnowledgeDocument> _courseDocuments;

        public string Name => "course_lookup";
        public string Description => "Looks up ONE named course's credits and instructor.";

        public CourseLookupTool(List<KnowledgeDocument> courseDocuments)
        {
            _courseDocuments = courseDocuments;
        }

        public bool CanHandle(string question)
        {
            return question.Contains("course", StringComparison.OrdinalIgnoreCase)
                || question.Contains("credit", StringComparison.OrdinalIgnoreCase)
                || question.Contains("instructor", StringComparison.OrdinalIgnoreCase);
        }

        public string Execute(string question)
        {
            foreach (var doc in _courseDocuments)
            {
                // Reuses the course's own name as the match key — no
                // separate name list needed, unlike the student tool
                // (courses are few and their names are already unique
                // and stable, so this simpler check is honest, not lazy;
                // the difference is disclosed in the IG next to this
                // method too).
                var courseName = doc.Text.Split(" is a ")[0];
                if (question.Contains(courseName, StringComparison.OrdinalIgnoreCase))
                {
                    return doc.Text;
                }
            }

            return "I can look up a specific course, but I couldn't find a matching course " +
                   "name in the question.";
        }
    }
}
