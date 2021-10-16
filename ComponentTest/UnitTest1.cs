using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smart_CSVReader;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComponentTest
{
    [TestClass]
    public class UnitTest1
    {
        public List<string[]> results = new List<string[]>
            {
                new string[] {"Author", "Title", "Description", "Year" },
                new string[] {"George Orwell", "1984", "The book is about what Orwell thought the world could have looked like in the year 1984. It describes a terrifying world where governments control and watch everyone's lives. The main character is Winston Smith. He lives in a country that is ruled by a powerful Party and its leader Big Brother, and dreams of changing this. He falls in love with Julia, who agrees with him, and is led into rebellion against the government.", "08.06.1949" },
                new string[] {"Tom Clancy", "Rainbow Six", "It is the second book to primarily focus on John Clark, one of the recurring characters in the Ryanverse, after Without Remorse (1993). Rainbow Six also features his son-in-law Domingo Ding Chavez, and explores the adventures of a multinational counter-terrorism unit that they formed, codenamed as Rainbow.", "01.08.1998"},
                new string[] {"Stanislaw Lem", "The Invincible", "A heavily-armed interstellar spacecraft called Invincible lands on the planet Regis III, which seems uninhabited and bleak, to investigate the loss of her sister ship, Condor. During the investigation, the crew finds evidence of a form of quasi-life, born through evolution of autonomous, self-replicating machines, apparently left behind by an alien civilization ship which landed on Regis III a very long time ago", "01.01.1964" },
                new string[] {"Lothar-Günther Buchheim", "Das Boot", " In autumn 1941, a German U-boat commander and his crew set out on yet another hazardous patrol in the Battle of the Atlantic. Over the coming weeks they brave the ocean's stormy waters and seek out British supply ships to destroy. But their targets travel in well-guarded convoys.", "01.01.1973" },
                new string[] {"Leo Tolstoy", "War and Peace", "The novel chronicles the French invasion of Russia and the impact of the Napoleonic era on Tsarist society through the stories of five Russian aristocratic families.", "01.01.1867" },
            };

        [TestMethod]
        public async Task ParseCSVwithHeader_Correct()
        {
            var test = await CSVReader.ParseCSVwithHeaderAsync<TestModel>("C:\\Users\\emre.rauhofer\\Documents\\Local_Projects\\Smart_CSVReader\\ComponentTest\\test.csv", ';');
            Assert.IsTrue(test.Item1);
            
            for (int i = 1; i < this.results.Count; i++)
            {
                Assert.AreEqual(this.results[i][0], test.Item2[i - 1].Author);
                Assert.AreEqual(this.results[i][1], test.Item2[i - 1].Title);
                Assert.AreEqual(this.results[i][2], test.Item2[i - 1].Description);
                Assert.AreEqual(this.results[i][3], test.Item2[i - 1].Year);
            }
        }

        [TestMethod]
        public async Task ParseCSV_WrongClass()
        {
            var test = await CSVReader.ParseCSVwithHeaderAsync<TestmodelConstructor>("C:\\Users\\emre.rauhofer\\Documents\\Local_Projects\\Smart_CSVReader\\ComponentTest\\test.csv", ';');
            Assert.IsFalse(test.Item1);
            Assert.AreEqual(0, test.Item2.Count);
        }

        [TestMethod]
        public async Task ParseCSV_Correct()
        {
            var test = await CSVReader.ParseCSVAsync<TestModel>("C:\\Users\\emre.rauhofer\\Documents\\Local_Projects\\Smart_CSVReader\\ComponentTest\\test2_noheader.csv",
                ';', new string[] { "Author", "Title", "Description", "Year" });
            Assert.IsTrue(test.Item1);

            for (int i = 1; i < this.results.Count; i++)
            {
                Assert.AreEqual(this.results[i][0], test.Item2[i - 1].Author);
                Assert.AreEqual(this.results[i][1], test.Item2[i - 1].Title);
                Assert.AreEqual(this.results[i][2], test.Item2[i - 1].Description);
                Assert.AreEqual(this.results[i][3], test.Item2[i - 1].Year);
            }
        }

        [TestMethod]
        public async Task ParseCSV_ClassWithSubClass()
        {
            var test = await CSVReader.ParseCSVAsync<TestModelSubClass>("C:\\Users\\emre.rauhofer\\Documents\\Local_Projects\\Smart_CSVReader\\ComponentTest\\test2_noheader.csv",
                ';', new string[] { "Author", "Title", "Description", "Year" });
            Assert.IsFalse(test.Item1);
            Assert.AreEqual(0, test.Item2.Count);
        }

        [TestMethod]
        public async Task ParseCSV_NoCSV()
        {
            var test = await CSVReader.ParseCSVAsync<TestModel>(string.Empty,
                ';', new string[] { "Author", "Title", "Description", "Year" });
            Assert.IsFalse(test.Item1);
            Assert.AreEqual(0, test.Item2.Count);
        }

        [TestMethod]
        public async Task ParseCSV_NoCSV2()
        {
            var test = await CSVReader.ParseCSVwithHeaderAsync<TestModel>(string.Empty,
                ';');
            Assert.IsFalse(test.Item1);
            Assert.AreEqual(0, test.Item2.Count);
        }
    }
}
