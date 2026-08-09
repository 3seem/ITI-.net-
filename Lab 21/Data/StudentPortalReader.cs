// =====================================================================
// StudentPortalReader — StudentPortalAI
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 21 — GenAI Fundamentals, RAG, AI Agents
//
// READ-ONLY access to the real, live ITI_StudentPortal database — the
// exact same database Sessions 13-20 built and every trainee's own
// StudentPortalWeb already connects to (Rule 38: never a fresh copy,
// never reset). This file issues plain SELECT statements only. It
// creates no table, runs no migration, and writes no row — there is
// nothing here for Session 22-that-doesn't-exist to worry about, because
// this is the last session of the track.
//
// Deliberately raw ADO.NET (Microsoft.Data.SqlClient), not EF Core.
// Today's new material is LLMs/RAG/agents — pulling EF into a brand
// new console project would be a second, unrelated new thing on an
// already-heavy day, and EF was already taught in full back in
// Sessions 13-14. Plain, parameterized SQL is enough for read-only work.
// =====================================================================
using Microsoft.Data.SqlClient;

namespace StudentPortalAI
{
    public class StudentPortalReader
    {
        // Byte-identical to Session 20's Program.cs connection string
        // (BUILD_PLAN.md §9) — same server, same database, same
        // credentials shape. Nothing about the environment changes today.
        public const string ConnectionString =
            "Data Source=.;Initial Catalog=ITI_StudentPortal;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";

        // Turns every real student row into one plain-English knowledge
        // document: "Ahmed Ali is a Year 2 student with a GPA of 3.62,
        // which places them in the First honour band."
        // Honour-band thresholds are copied EXACTLY from
        // StudentsController.Honours (Session 16/20's real, live code) —
        // first >= 3.5, second >= 3.0 and < 3.5, otherwise pass.
        public List<KnowledgeDocument> LoadStudentDocuments()
        {
            var docs = new List<KnowledgeDocument>();

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string sql =
                "SELECT Id, FullName, YearOfStudy, Gpa FROM dbo.Students ORDER BY Id;";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("Id"));
                string fullName = reader.GetString(reader.GetOrdinal("FullName"));
                int yearOfStudy = reader.GetInt32(reader.GetOrdinal("YearOfStudy"));
                double gpa = reader.GetDouble(reader.GetOrdinal("Gpa"));

                string band = HonourBandFor(gpa);

                string text =
                    $"{fullName} is a Year {yearOfStudy} student with a GPA of {gpa:F2}, " +
                    $"which places them in the {band} honour band.";

                docs.Add(new KnowledgeDocument
                {
                    SourceTable = "Students",
                    SourceId = id,
                    Text = text
                });
            }

            return docs;
        }

        // Turns every real course row (with its instructor) into one
        // plain-English knowledge document.
        public List<KnowledgeDocument> LoadCourseDocuments()
        {
            var docs = new List<KnowledgeDocument>();

            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string sql = @"
                SELECT c.Id, c.CourseName, c.Credits, i.FullName AS InstructorName
                FROM dbo.Courses c
                JOIN dbo.Instructors i ON i.Id = c.InstructorId
                ORDER BY c.Id;";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("Id"));
                string courseName = reader.GetString(reader.GetOrdinal("CourseName"));
                int credits = reader.GetInt32(reader.GetOrdinal("Credits"));
                string instructorName = reader.GetString(reader.GetOrdinal("InstructorName"));

                string text =
                    $"{courseName} is a {credits}-credit course taught by {instructorName}.";

                docs.Add(new KnowledgeDocument
                {
                    SourceTable = "Courses",
                    SourceId = id,
                    Text = text
                });
            }

            return docs;
        }

        // Looks up ONE real student by exact full name (case-insensitive).
        // Used directly by the agent's StudentLookupTool — not through
        // retrieval, because a tool call is a precise lookup, not a
        // similarity search. That distinction IS the lesson in Block 8.
        public (bool Found, string FullName, int YearOfStudy, double Gpa) FindStudentByName(string name)
        {
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            const string sql =
                "SELECT TOP 1 FullName, YearOfStudy, Gpa FROM dbo.Students WHERE FullName = @name;";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", name);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (true,
                    reader.GetString(reader.GetOrdinal("FullName")),
                    reader.GetInt32(reader.GetOrdinal("YearOfStudy")),
                    reader.GetDouble(reader.GetOrdinal("Gpa")));
            }

            return (false, "", 0, 0.0);
        }

        public static string HonourBandFor(double gpa)
        {
            if (gpa >= 3.5) return "First";
            if (gpa >= 3.0) return "Second";
            return "Pass";
        }
    }
}
