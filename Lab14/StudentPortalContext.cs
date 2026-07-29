using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab_13
{
    public class StudentPortalContext : DbContext
    {
        public DbSet<Student> Students { get; set; } // Represents the Students table
        public DbSet<Course> Courses { get; set; }
        public DbSet<Instructor> Instructors { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                    "Server=DESKTOP-3D31ED4;Database=ITI_StudentPortal;Trusted_Connection=True;TrustServerCertificate=True"
                )
                .LogTo(Console.WriteLine , LogLevel.Information)
                .EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Part D: Fluent API 
            modelBuilder.Entity<Student>()
                .Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(100);

            // Part E: real Instructor <-> Course relationship
            // Lab ID 27 -> 27 % 2 = 1 -> SetNull
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.SetNull);
        }

    }
}
