

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace lab_13
{
    // =================================================================
    // THE MODEL — already written, used by BOTH halves of today.
    //
    // These are deliberately simpler than Session 11's Person/IPrintable
    // hierarchy: no base class, no interfaces, no validating setters.
    // A table row has no concept of an abstract base class. The SAME
    // class you query in memory this morning becomes a SQL Server table
    // this afternoon, with nothing about it changing.
    //
    // `Id` becomes an auto-incrementing PRIMARY KEY purely by EF's
    // naming convention — nobody writes PRIMARY KEY anywhere.
    // =================================================================
    public class Student // class = table
    {
       
        public int Id { get; set; } // property = column  // StudentId or Id

        // Part D Data Annotation constraints
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = ""; 
        public int YearOfStudy { get; set; }
        public double Gpa { get; set; }

        public string? Email { get; set; } // Part G Lab ID 27 mod 3 = 0
    }

  
}
