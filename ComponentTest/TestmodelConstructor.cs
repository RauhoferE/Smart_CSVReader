using System;
using System.Collections.Generic;
using System.Text;

namespace ComponentTest
{
    public class TestmodelConstructor
    {
        public TestmodelConstructor(string author, string title, string description, string year)
        {
            Author = author;
            Title = title;
            Description = description;
            Year = year;
        }

        public string Author { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Year { get; set; }
    }
}
