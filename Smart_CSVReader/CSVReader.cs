namespace Smart_CSVReader
{
    using Smart_CSVReader.Attributes;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple CSV Reader that can pass the lines into an object.
    /// </summary>
    public static class CSVReader
    {
        /// <summary>
        /// Parses a CSV file with a header into the given class.
        /// </summary>
        /// <typeparam name="T"> The class to be parsed. </typeparam>
        /// <param name="csv"> The path of the CSV file. </param>
        /// <param name="delimiter"> The delimiter used for the CSV. </param>
        /// <returns> It returns a tripple with (isPrased, parsed objects, error message). </returns>
        public static async Task<(bool, List<T>, string)> ParseCSVwithHeaderAsync<T>(string csv, char delimiter) where T:class
        {
            (bool, List<string[]>) res = await ReadCSVAsync(csv, delimiter);

            if (!res.Item1)
            {
                return (false, new List<T>(), "Could'nt read csv file.");
            }

            return await ParseLinesWithHeaderAsync<T>(res.Item2);
        }

        /// <summary>
        /// Parses a CSV file without a header into the given class.
        /// </summary>
        /// <typeparam name="T"> The class to be parsed. </typeparam>
        /// <param name="csv"> The path of the CSV file. </param>
        /// <param name="delimiter"> The delimiter used for the CSV. </param>
        /// <returns> It returns a tripple with (isPrased, parsed objects, error message). </returns>
        public static async Task<(bool, List<T>, string)> ParseCSVAsync<T>(string csv, char delimiter, string[] header) where T : class
        {
            (bool, List<string[]>) res = await ReadCSVAsync(csv, delimiter);

            if (!res.Item1)
            {
                return (false, new List<T>(), "Could'nt read csv file.");
            }

            return await ParseLinesAsync<T>(res.Item2, header);
        }

        /// <summary>
        /// This method reads all lines from a csv files and split the columns.
        /// </summary>
        /// <param name="csv"> The path to the csv file. </param>
        /// <param name="delimiter"> The delimiter character. </param>
        /// <returns> It returns a tuple that can contain true and the list of lines or false and an empty list. </returns>
        private static async Task<(bool, List<string[]>)> ReadCSVAsync(string csv, char delimiter)
        {
            List<string[]> lines = new List<string[]>();

            try
            {    
                using (StreamReader reader = new StreamReader(csv))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] vals = line.Split(delimiter);

                        for (int i = 0; i < vals.Length; i++)
                        {
                            vals[i] = vals[i].Trim();
                            
                            if (vals[i][0] == '\"' || vals[i][0] == '\'')
                            {
                                vals[i] = vals[i].Substring(1, vals[i].Length - 2);
                            }
                        }

                        lines.Add(vals);
                    };
                }
            }
            catch (Exception e)
            {
                return (false, lines);
            }

            return (true, lines);
        }

        private static Task<Dictionary<int, string>> MapPropertiesToHeaderPosition(PropertyInfo[] properties, string[] headers)
        {
            Dictionary<int, string> indexToPropertyDict = new Dictionary<int, string>();
            for (int i = 0; i < headers.Length; i++)
            {
                foreach (var property in properties)
                {
                    CSVHeaderNameAttribute nameAttribute = (CSVHeaderNameAttribute)property.GetCustomAttribute(typeof(CSVHeaderNameAttribute));
                    CSVHeaderIndexAttribute indexAttribute = (CSVHeaderIndexAttribute)property.GetCustomAttribute(typeof(CSVHeaderIndexAttribute));

                    if (nameAttribute != null && nameAttribute.GetHeaderName() == headers[i])
                    {
                        indexToPropertyDict.Add(i, property.Name);
                    }
                    else if (indexAttribute != null && i == indexAttribute.GetIndex())
                    {
                        indexToPropertyDict.Add(i, property.Name);
                    }
                    else if (property.Name == headers[i])
                    {
                        indexToPropertyDict.Add(i, property.Name);
                    }
                }
            }

            return Task.FromResult(indexToPropertyDict);
        }

        /// <summary>
        /// This method parses the array of lines with a header into the given type.
        /// </summary>
        /// <typeparam name="T"> The class to be parsed. </typeparam>
        /// <param name="lines"> The lines of the CSV file. </param>
        /// <returns> It returns a tuple that can contain true and the list of lines or false and an empty list. </returns>
        private static async Task<(bool, List<T>, string)> ParseLinesWithHeaderAsync<T>(List<string[]> lines) where T : class
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            Dictionary<int, string> indexToPropertyDict = await MapPropertiesToHeaderPosition(properties, lines[0]);

            lines.RemoveAt(0);
            return await CreateObjectListFromCSV<T>(lines, indexToPropertyDict);
        }

        /// <summary>
        /// This method creates a list of objects from every line in the csv file.
        /// </summary>
        /// <typeparam name="T"> The class that the lines are converted to. </typeparam>
        /// <param name="lines"> The lines of the csv file. </param>
        /// <param name="indexToPropertyDict"> The dictionary that maps a headernumber to a class property. </param>
        /// <returns> It returns a triple that can contain the list with the requested objects or an empty list and an error message. </returns>
        private static async Task<(bool, List<T>, string)> CreateObjectListFromCSV<T>(List<string[]> lines, Dictionary<int, string> indexToPropertyDict) where T : class
        {
            List<T> resultObjects = new List<T>();

            for (int i = 0; i < lines.Count; i++)
            {
                try
                {
                    T newCreated = (T)Activator.CreateInstance(typeof(T));

                    for (int j = 0; j < indexToPropertyDict.Count; j++)
                    {
                        MethodInfo parser = newCreated.GetType().GetProperty(indexToPropertyDict.GetValueOrDefault(j)).PropertyType
                            .GetMethod("Parse", new Type[] { typeof(string) });
                        PropertyInfo property = newCreated.GetType().GetProperty(indexToPropertyDict.GetValueOrDefault(j));
                        if (parser != null)
                        {
                            property.SetValue(newCreated, parser.Invoke(property, new object[] { lines[i][j] }));
                        }
                        else
                        {
                            property.SetValue(newCreated, lines[i][j]);
                        }
                    }

                    resultObjects.Add(newCreated);
                }
                catch (Exception e)
                {
                    return (false, new List<T>(), e.Message);
                }
            }

            return (true, resultObjects, string.Empty);
        }

        /// <summary>
        /// This method parses the array of lines without a header into the given type.
        /// </summary>
        /// <typeparam name="T"> The class to be parsed. </typeparam>
        /// <param name="lines"> The lines of the CSV file. </param>
        /// <param name="header"> The headers of the CSV file in the correct order. </param>
        /// <returns> It returns a tuple that can contain true and the list of lines or false and an empty list. </returns>
        private static async Task<(bool, List<T>, string)> ParseLinesAsync<T>(List<string[]> lines, string[] header) where T : class
        {
            var properties = typeof(T).GetProperties();
            Dictionary<int, string> indexToPropertyDict = await MapPropertiesToHeaderPosition(properties, header);

            List<T> resultObjects = new List<T>();
            return await CreateObjectListFromCSV<T>(lines, indexToPropertyDict);
        }
    }
}
