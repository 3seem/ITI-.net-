// =====================================================================
// StudentPortalContext — CARRIED FORWARD FROM SESSION 14 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
//
// This file arrives EXACTLY as Session 14 left it. Same class name, same
// entities, same Fluent API configuration, same hardcoded connection
// string inside OnConfiguring. Nothing here has been changed for you.
//
// That is deliberate. Today's whole point is that this class does not
// have to be rewritten to work on the web — it has to be handed over to
// somebody else to construct. TODO 1 and TODO 2 make exactly that one
// change, and nothing else in this file moves.
//
// ⚠️ MIGRATION OWNERSHIP — read this before you touch the database:
//    The Session 14 CONSOLE project is still the owner of this
//    database's migrations. Its Migrations/ folder holds the history
//    that produced the tables you are about to read.
//    This web project deliberately has NO Migrations/ folder, and you
//    must NOT run Add-Migration or Update-Database from it today. It
//    would have no history to build on and would try to create tables
//    that already exist.
//    Today the web app READS a database that yesterday already built.
// =====================================================================

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab15.Models
{
    // =================================================================
    // THE ENTITIES — unchanged from Session 14.
    // Student carries the [Required]/[MaxLength] annotations added in
    // Session 14 Block 2. Course carries the InstructorId foreign key
    // and Instructor navigation property added in Block 3, and
    // Instructor carries the Courses collection on the other end.
    // =================================================================
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearOfStudy { get; set; }

        public double Gpa { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string CourseName { get; set; } = "";

        public int Credits { get; set; }

        public int InstructorId { get; set; }

        public Instructor Instructor { get; set; } = null!;
    }

    public class Instructor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        public int YearsOfExperience { get; set; }

        public List<Course> Courses { get; set; } = new();
    }

    // =================================================================
    // THE CONTEXT — also unchanged from Session 14, for now.
    // =================================================================
    public class StudentPortalContext : DbContext
    {
        
        //protected StudentPortalContext()
        //{
        //}

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        // TODO 1: Add a constructor to this class that accepts one
        //         parameter: the generic "here is how you were
        //         configured" type that EF Core provides, with THIS
        //         context class as its type argument. The constructor
        //         body stays empty — instead, pass that parameter
        //         straight up to the base class using the base-
        //         constructor-chaining syntax you learned in Session 10.
        //         This single line is what makes the class constructible
        //         by somebody other than you: whoever creates it now has
        //         to supply the configuration from outside.

        //Part C

        public StudentPortalContext(DbContextOptions<StudentPortalContext> options) : base(options)
        {
        }


        // TODO 2: Once TODO 1 is in place, DELETE the entire
        //         OnConfiguring method below. Read it one more time
        //         first, and notice exactly what you are deleting: the
        //         only copy of the connection string, and the assumption
        //         that this class configures itself. From this point on,
        //         it cannot connect to anything unless somebody hands it
        //         a configuration — which is the whole idea.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Session 14 Block 2 — Fluent API wins over annotations.
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // Session 14 Block 3 — the real relationship. Restrict means
            // the database refuses to delete an instructor who still has
            // courses, rather than silently deleting the courses too.
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
