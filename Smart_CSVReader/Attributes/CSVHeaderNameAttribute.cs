using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_CSVReader.Attributes
{
    /// <summary>
    /// The custom attribute that can assign a header name to a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CSVHeaderNameAttribute : Attribute
    {
        /// <summary>
        /// The name of the header in the csv file.
        /// </summary>
        private string headerName;

        /// <summary>
        /// Creates a new attribute with the given header name.
        /// </summary>
        /// <param name="headerName"> The name of the header in the csv file. </param>
        public CSVHeaderNameAttribute(string headerName)
        {
            this.headerName = headerName;
        }

        /// <summary>
        /// This method gets the current name of the header.
        /// </summary>
        /// <returns> It returns the current name of the header as a string. </returns>
        public string GetHeaderName()
        {
            return this.headerName;
        }
    }
}
