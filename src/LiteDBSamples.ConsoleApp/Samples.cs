using LiteDB;

namespace LiteDBSamples.ConsoleApp
{
    public class Person
    {

    }

    class Samples
    {
        public Samples(string path)
        {
            //BsonMapper.Global
            string sampleCollectionName = "MyCollection1";
            LiteDB.ILiteDatabase db = new LiteDB.LiteDatabase(path);
            if (db.CollectionExists(sampleCollectionName))
            {
                db.DropCollection(sampleCollectionName);
                db.GetCollection<Person>();
            }
        }
    }
}
