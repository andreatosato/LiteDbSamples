using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDBSamples.ConsoleApp.Models
{
    public class Student
    {
        [BsonCtor]
        public Student(ObjectId id, string studentId)
        {
            Id = id;
            StudentId = studentId;
        }

        public ObjectId Id { get; set; }
        public string StudentId { get; set; }
        [BsonField("studentName")]
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public Uri Photo { get; set; }
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
