// InstructorLookupTool — StudentPortalAI
// Lab 21, Part A3 — Personal Fingerprint (Lab ID 27, ODD assignment)


//Part B1
//an `AssignInstructorTool` that updates which instructor is assigned to a course the one
//guardrail I'd add first is never let it perform a direct `UPDATE` on `dbo.Courses`.
//Instead, it should create a pending `InstructorChangeRequest` record that a human
//has to approve before the real assignment changes


//Part B2

//The `StudentPortalWeb` ASP.NET Core MVC/Razor Pages site, with conventional and attribute
// routing (Lab 16)


//the `StudentsController` `Edit` action pair, with the
//guard - clause - before - save pattern and a proper redirect (not a view return) on a
//   successful POST(Lab 17)


//the write side of `Enrollment`: a full `EnrollmentsController` built
// from scratch (GET + POST) (Lab 19)



//`StudentPortalAI` — the RAG pipeline and agent built in Session 21, including my own
//`InstructorLookupTool`, all answering real questions grounded in the real
//`ITI_StudentPortal` data


namespace StudentPortalAI
{
    public class InstructorLookupTool : ITool
    {
        private readonly List<KnowledgeDocument> _courseDocuments;

        public string Name => "instructor_lookup";
        public string Description => "Looks up who teaches ONE named course.";

        public InstructorLookupTool(List<KnowledgeDocument> courseDocuments)
        {
            _courseDocuments = courseDocuments;
        }

        public bool CanHandle(string question)
        {
            return question.Contains("who teaches", StringComparison.OrdinalIgnoreCase)
                || question.Contains("instructor", StringComparison.OrdinalIgnoreCase);
        }

        public string Execute(string question)
        {
            foreach (var doc in _courseDocuments)
            {
                var courseName = doc.Text.Split(" is a ")[0];
                if (question.Contains(courseName, StringComparison.OrdinalIgnoreCase))
                {
                    var parts = doc.Text.Split("taught by", (int)StringComparison.OrdinalIgnoreCase);
                    if (parts.Length > 1)
                    {
                        var instructorName = parts[1].Trim().TrimEnd('.');
                        return $"{instructorName} teaches {courseName}.";
                    }

                    return doc.Text;
                }
            }

            return "I can look up who teaches a specific course, but I couldn't find a " +
                   "matching course name in the question.";
        }
    }
}