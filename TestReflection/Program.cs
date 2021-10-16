using Smart_CSVReader;
using System;

namespace TestReflection
{
    class Program
    {
        static void Main(string[] args)
        {
            TestModelSubClass newCreated = new TestModelSubClass();
            //var c = newCreated.GetType().GetProperty("Year").PropertyType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(
                //newCreated.GetType().GetProperty("Year"), new object[] { "2020" });
            var res = CSVReader.ParseCSVwithHeaderAsync<TestModelSubClass>("C:\\Users\\emre.rauhofer\\Documents\\Local_Projects\\Smart_CSVReader\\TestReflection\\test.csv", ';').Result;
            //var c = newCreated.GetType().GetProperty("Year").PropertyType.GetMethods();
            foreach (var item in res.Item2)
            {
                Console.WriteLine(item.Year.ToString());
            }
        }
    }
}
