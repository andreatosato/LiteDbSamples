using LiteDB;
using LiteDBSamples.ConsoleApp.DataGenerator;
using LiteDBSamples.ConsoleApp.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiteDBSamples.ConsoleApp
{
    class Program
    {
        private static string filePath = @$"{AppContext.BaseDirectory}consoleapp.db";
        static async Task Main(string[] args)
        {
            CreateFile();
            Console.ForegroundColor = ConsoleColor.Green;
            BsonDocumentExample();
            
            await Task.Delay(10_000);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;

            CreateFile();
            MapperExample();

            await Task.Delay(10_000);
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;

            CreateFile();
            CreateBogusExample();

            Console.Read();
        }

        private static void BsonDocumentExample()
        {
            using (var db = new LiteDatabase(filePath))
            {
                var customer = new BsonDocument();
                customer["_id"] = ObjectId.NewObjectId();
                customer["Name"] = "John Doe";
                customer["CreateDate"] = DateTime.Now;
                customer["Phones"] = new BsonArray { "8000-0000", "9000-000" };
                customer["IsActive"] = true;
                customer["IsAdmin"] = new BsonValue(true);
                customer["Address"] = new BsonDocument
                {
                    ["Street"] = "Av. Protasio Alves",
                };
                customer["Address"]["Number"] = "1331";
                var col = db.GetCollection("customer");
                col.Insert(customer);

                var customersRead = col.FindAll();
                foreach (var c in customersRead)
                {
                    Console.WriteLine(c.ToString());
                    Console.WriteLine();
                }
            }
        }

        class MyEntity
        {
            public Guid MyId { get; set; }
            public int Number { get; set; } = new Random().Next(1, 1000);
            public string Name { get; set; } = "Andrea Tosato";
            public override string ToString()
            {
                return $"{MyId}-{Number}-{Name}";
            }
        }

        private static void MapperExample()
        {
            // Definition create one-shot
            var mapper = BsonMapper.Global;
            mapper.Entity<MyEntity>()
                .Id(x => x.MyId, true) // set your document ID
                .Ignore(x => x.Number) // ignore this property (do not store)
                .Field(x => x.Name, "entity_name"); // rename document field

            using (var db = new LiteDatabase(filePath))
            {
                var col = db.GetCollection<MyEntity>("my_collection_name", BsonAutoId.Guid);
                col.Insert(new MyEntity());
                for (int i = 0; i < 50; i++)
                {
                    var e = new MyEntity() { Name = i.ToString() };
                    col.Insert(e);
                }
                db.Checkpoint();


                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var r1 = col.FindOne(t => t.Name == "Andrea Tosato");
                stopwatch.Stop();
                Console.WriteLine("Read: {0}", stopwatch.ElapsedMilliseconds);
                Console.WriteLine("Read: {0}", r1);



                col.EnsureIndex(t => t.Name);
                stopwatch.Reset();
                var r2 = col.FindOne(t => t.Name == "Andrea Tosato");
                stopwatch.Stop();
                Console.WriteLine("Read: {0}", stopwatch.ElapsedMilliseconds);
                Console.WriteLine("Read: {0}", r2);
            }
        }

        private static void CreateBogusExample()
        {
            BsonMapper.Global.RegisterType<Uri>
                (
                    serialize: (uri) => uri.AbsoluteUri,
                    deserialize: (bson) => new Uri(bson.AsString)
                );

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

        private static void CreateFile()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
