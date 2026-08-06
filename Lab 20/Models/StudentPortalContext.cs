// =====================================================================
// StudentPortalContext — CARRIED FORWARD FROM SESSION 19 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 20 — ASP.NET Core Razor Pages
//
// ⚠️ NOT ONE LINE OF THIS FILE CHANGES TODAY. No new entity, no new
//    property, no new Fluent API call, and — this is the point Block 5
//    comes back to — NO NEW MIGRATION. Today adds a second way to SHOW
//    this data. It does not touch what the data IS.
//
//    Student · Course · Instructor · Enrollment, the two Cascade
//    relationships and the composite unique index on
//    (StudentId, CourseId) are exactly as the room left them on Aug 5.
// =====================================================================
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentPortalWeb.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [Range(1,4,ErrorMessage = "Year of study must be between 1 and 4.")]
        public int YearOfStudy { get; set; }

        [Range(0.0,4.0,ErrorMessage = "GPA must be between 0.0 and 4.0.")]
        public double Gpa { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new();
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

        public List<Enrollment> Enrollments { get; set; } = new();
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

    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        [Range(0.0, 4.0, ErrorMessage = "Grade must be between 0.0 and 4.0")]
        public double? Grade { get; set; }
    }
    public class StudentPortalContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public StudentPortalContext(DbContextOptions<StudentPortalContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s=>s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

        }
    }
}