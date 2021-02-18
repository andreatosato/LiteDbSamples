using Bogus;
using LiteDBSamples.ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiteDBSamples.ConsoleApp.DataGenerator
{
    public static class SampleGenerator
    {
        public static List<Course> GenerateCourse(int numbers)
        {
            string[] courses = new [] { "Analisi A", "Analisi B", "Fisica A"};
            var courseGenerator = new Faker<Course>()
                .RuleFor(c => c.Name, f => f.PickRandom(courses))
                .RuleFor(c => c.CFU, f => f.Random.Number(0, 12));

            return courseGenerator.Generate(numbers);
        }

        public static List<Student> GenerateStudents(List<Course> courses, int numbers)
        {
            var studentGenerator = new Faker<Student>()
                .CustomInstantiator((f) => new Student(LiteDB.ObjectId.NewObjectId(), f.Random.AlphaNumeric(8).ToUpper()))
                .RuleFor(s => s.Name, f => f.Name.FirstName())
                .RuleFor(s => s.Surname, f => f.Name.LastName())
                .RuleFor(s => s.Photo, f => new Uri(f.Internet.Avatar()))
                .RuleFor(s => s.Birthday, f => f.Date.Past(18, DateTime.Now.AddYears(-100)));
            
            var students = studentGenerator.Generate(numbers);

            var studentCourseGenerator = new Faker<StudentCourse>()
                .RuleFor(s => s.Course, f => f.PickRandom(courses))
                .RuleFor(s => s.Vote, f => f.Random.Int(18, 30))
                .RuleFor(s => s.Accepted, (f, sc) => f.Random.Bool((float)(1 - (sc.Vote * 0.033))))
                .RuleFor(s => s.ExamDate, f => f.Date.Soon(1, DateTime.Now.AddYears(10)))
                .RuleFor(s => s.AcademicYear, f => new DateTime(f.Date.Soon(1, DateTime.Now.AddYears(10)).Year,1,1));

            foreach (var s in students)
            {
                var studentCourse = studentCourseGenerator.Generate(new Random().Next(0, 50));
                s.Courses = studentCourse;
            }
            return students;
        }
    }
}
