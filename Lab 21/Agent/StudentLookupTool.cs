// =====================================================================
// StudentLookupTool — StudentPortalAI
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// A PRECISE tool, not a similarity search. If the question names a real
// student, this queries the exact row directly — no embeddings, no
// "closest match," a real WHERE clause. This is the concrete difference
// Block 8 draws between "retrieval" (Block 3 — approximate, ranked) and
// "a tool call" (exact, structured) — both are real, both matter, and
// a real agent chooses between them per question.
// =====================================================================
namespace StudentPortalAI
{
    public class StudentLookupTool : ITool
    {
        private readonly StudentPortalReader _reader;

        public string Name => "student_lookup";
        public string Description => "Looks up ONE named student's exact year and GPA by full name.";

        public StudentLookupTool(StudentPortalReader reader)
        {
            _reader = reader;
        }

        public bool CanHandle(string question)
        {
            return question.Contains("student", StringComparison.OrdinalIgnoreCase)
                || question.Contains("honour", StringComparison.OrdinalIgnoreCase)
                || question.Contains("honor", StringComparison.OrdinalIgnoreCase)
                || question.Contains("gpa", StringComparison.OrdinalIgnoreCase);
        }

        // Very small, deliberately simple name extraction: tries every
        // student full name we know about and checks whether it appears
        // in the question. A real agent would let the MODEL extract the
        // name as a structured function-call argument — disclosed in
        // the IG next to this exact method, per Rule 17.
        public string Execute(string question)
        {
            foreach (var candidate in KnownStudentNames)
            {
                if (question.Contains(candidate, StringComparison.OrdinalIgnoreCase))
                {
                    var (found, fullName, yearOfStudy, gpa) = _reader.FindStudentByName(candidate);
                    if (found)
                    {
                        string band = StudentPortalReader.HonourBandFor(gpa);
                        return $"{fullName} is a Year {yearOfStudy} student with a GPA of {gpa:F2}, " +
                               $"which places them in the {band} honour band.";
                    }
                }
            }

            return "I can look up a specific student, but I couldn't find a matching name " +
                   "in the question.";
        }

        // Populated at startup from the real database (see Program.cs)
        // so this tool always knows about every REAL student currently
        // in ITI_StudentPortal — never a hardcoded, invented list.
        public List<string> KnownStudentNames { get; set; } = new();
    }
}
