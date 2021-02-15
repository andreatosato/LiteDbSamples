using LiteDB;
using LiteDBSamples.ConsoleApp.DataGenerator;
using LiteDBSamples.ConsoleApp.Models;
using System;
using System.IO;
using System.Text.Json;

namespace LiteDBSamples.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Console App!");
            string filePath = @$"{AppContext.BaseDirectory}consoleapp.db";
            if (File.Exists(filePath))
                File.Delete(filePath);

            using (var db = new LiteDatabase(filePath))
            {
                var courseCollection = db.GetCollection<Course>("MyCourses", BsonAutoId.Int64);
                var studentCollection = db.GetCollection<Student>();
                var courses = SampleGenerator.GenerateCourse(50);
                var students = SampleGenerator.GenerateStudents(courses, 200);
                var inserted = courseCollection.InsertBulk(courses, batchSize: 1000);
                studentCollection.InsertBulk(students);
                Console.WriteLine($"Inserted: {inserted}");

                var createIndexOperation = studentCollection.EnsureIndex(x => x.StudentId);
                Console.WriteLine($"Index created: {createIndexOperation}");


                BsonMapper.Global.Entity<Student>().DbRef(x => x.Courses, "courses");
                // [BsonRef("courses")] 
                Console.WriteLine("Set reference");

                var studentsIncludeCourses = studentCollection.Include(s => s.Courses).FindAll();
                foreach (var sc in studentsIncludeCourses)
                {
                    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(sc, new JsonSerializerOptions() { WriteIndented = true }));
                }

                courses = SampleGenerator.GenerateCourse(20);
                inserted = courseCollection.InsertBulk(courses);
                Console.WriteLine($"Inserted: {inserted}");


                var countElements = courseCollection.LongCount();
                Console.WriteLine($"Element in Collection: {countElements}");

                db.Checkpoint();
                db.Dispose();
            }
        }
    }
}
