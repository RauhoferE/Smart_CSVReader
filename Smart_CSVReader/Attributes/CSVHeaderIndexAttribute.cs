using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_CSVReader.Attributes
{
    /// <summary>
    /// The custom attribute that can assign an index header number to a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CSVHeaderIndexAttribute : Attribute
    {
        /// <summary>
        /// The index number of the header in the csv file.
        /// </summary>
        private int index;

        /// <summary>
        /// Creates a new attribute with the index number.
        /// </summary>
        /// <param name="index"> The index number of the header in the csv file. </param>
        public CSVHeaderIndexAttribute(int index)
        {
            this.index = index;
        }

        /// <summary>
        /// This method gets the current index of the header.
        /// </summary>
        /// <returns> It returns the current index number of the header as an integer. </returns>
        public int GetIndex()
        {
            return this.index;
        }
    }
}
