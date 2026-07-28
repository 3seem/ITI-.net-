// =====================================================================
// StudentPortalConsole — SESSION PROJECT (Style Guide Rule 20/35/39/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 13 — LINQ Part 2 + EF Core (Code First)
//
// THIS PROJECT IS DAY-READY (Rule 39). The model classes and all seed
// data are already present as REAL, WORKING, RUNNABLE code, and the EF
// Core packages are already referenced. Open it, press run, and it
// builds and prints the Session 12 warm-up chain immediately — before a
// single TODO is done.
//
// Only TODAY'S NEW content is left as TODOs. Each TODO sits exactly
// where its code will be written (Rule 40), and the numbers run
// strictly top-to-bottom through this file in the same order the
// lecture teaches them:
//
//   TODO 1      Block 3 — your own LINQ extension method (top-level)
//   TODO 2-3    Blocks 4-5 — the DbContext
//   TODO 4-6    Block 1 (aggregates), inside Main
//   TODO 7-11   Block 2 (GroupBy, Join), inside Main
//   TODO 12-16  Block 3 (deferred execution), inside Main
//   TODO 17-19  Blocks 4-5 (EF Core queries), inside Main
//
// ⚠️ WHY TODO 1 IS AT THE TOP RATHER THAN WITH BLOCK 3'S OTHER TODOs:
//   C# requires extension methods in a top-level static class (CS1109),
//   so it cannot sit inside Program next to the code that uses it. It is
//   numbered 1 because it is physically first (Rule 40); teach it when
//   you reach Block 3, and TODO 16 is where you actually call it.
//
// ⚠️ BEFORE THE EF TODOs WILL WORK:
//   1. Run Application/Session13_PreInit.sql in SSMS.
//   2. Check TODO 3's connection string matches YOUR server.
//   3. Package Manager Console:  Add-Migration InitialCreate
//                                Update-Database
//
// For the full, correct, runnable version (do NOT peek until you've
// tried it yourself, or you're checking your own work), see:
// ../StudentPortalConsole_Complete/Course.cs
// =====================================================================

namespace lab_13
{
    public class Course
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = "";
        public int Credits { get; set; }
    }

    
}
