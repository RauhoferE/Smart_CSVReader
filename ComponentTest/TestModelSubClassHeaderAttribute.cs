using Smart_CSVReader.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComponentTest
{
    public class TestModelSubClassHeaderAttribute
    {
        [CSVHeaderNameAttribute("Author")]
        public string AuthorProp { get; set; }

        public string Title { get; set; }

        [CSVHeaderNameAttribute("Description")]
        public string DescriptionProp { get; set; }

        public DateTime Year { get; set; }
    }
}
