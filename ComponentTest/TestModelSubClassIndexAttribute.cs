using System;
using Smart_CSVReader.Attributes;
using System.Collections.Generic;
using System.Text;

namespace ComponentTest
{
    class TestModelSubClassIndexAttribute
    {
        [CSVHeaderIndex(0)]
        public string AuthorProp { get; set; }

        public string Title { get; set; }

        [CSVHeaderIndex(2)]
        public string DescriptionProp { get; set; }

        public DateTime Year { get; set; }
    }
}
