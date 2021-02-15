using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDBSamples.ConsoleApp.Models
{
    public class Student
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public List<StudentCourse> Courses { get; set; }
    }

    public class StudentCourse
    {
        public Course Course { get; set; }
        public int Vote { get; set; }
        public bool Accepted { get; set; }
        public DateTime ExamDate { get; set; }
        public DateTime AcademicYear { get; set; }
    }
}
