namespace Smart_CSVReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
            var res = await ReadCSVAsync(csv, delimiter);

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
            var res = await ReadCSVAsync(csv, delimiter);

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
        private static Task<(bool, List<string[]>)> ReadCSVAsync(string csv, char delimiter)
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
                return Task.FromResult((false, lines));
            }

            return Task.FromResult((true, lines));
        }

        /// <summary>
        /// This method parses the array of lines with a header into the given type.
        /// </summary>
        /// <typeparam name="T"> The class to be parsed. </typeparam>
        /// <param name="lines"> The lines of the CSV file. </param>
        /// <returns> It returns a tuple that can contain true and the list of lines or false and an empty list. </returns>
        private static Task<(bool, List<T>, string)> ParseLinesWithHeaderAsync<T>(List<string[]> lines) where T : class
        {
            var properties = typeof(T).GetProperties();
            Dictionary<int, string> indexToPropertyDict = new Dictionary<int, string>();
            for (int i = 0; i < lines[0].Length; i++)
            {
                foreach (var property in properties)
                {
                    if (property.Name == lines[0][i])
                    {
                        indexToPropertyDict.Add(i, property.Name);
                    }
                }
            }

            List<T> resultObjects = new List<T>();
            for (int i = 1; i < lines.Count; i++)
            {
                try
                {
                    T newCreated = (T)Activator.CreateInstance(typeof(T));

                    for (int j = 0; j < indexToPropertyDict.Count; j++)
                    {
                        var hasParser = newCreated.GetType().GetProperty(indexToPropertyDict.GetValueOrDefault(j)).PropertyType.GetMethod("Parse", new Type[] { typeof(string) });
                        if (hasParser != null)
                        {
                            newCreated.GetType().GetProperty(indexToPropertyDict.GetValueOrDefault(j)).SetValue(newCreated, 
                                newCreated.GetType().GetProperty(indexToPropertyDict.GetValueOrDefault(j)).PropertyType.GetMethod("Parse", new Type[] { typeof(string) }).Invoke(
                                    newCreated.GetType().GetProperty(indexToPropertyDict.GetValueOrDefault(j)), new object[] { lines[i][j] }));
                        }
                        else
                        {
                            newCreated.GetType().GetProperty(indexToPropertyDict.GetValueOrDefault(j)).SetValue(newCreated, lines[i][j]);
                        }
                    }

                    resultObjects.Add(newCreated);
                }
                catch (Exception e)
                {
                    return Task.FromResult((false, new List<T>(), e.Message));
                }

            }

            return Task.FromResult((true, resultObjects, string.Empty));
        }

        /// <summary>
        /// This method parses the array of lines without a header into the given type.
        /// </summary>
        /// <typeparam name="T"> The class to be parsed. </typeparam>
        /// <param name="lines"> The lines of the CSV file. </param>
        /// <returns> It returns a tuple that can contain true and the list of lines or false and an empty list. </returns>
        private static Task<(bool, List<T>, string)> ParseLinesAsync<T>(List<string[]> lines, string[] header) where T : class
        {
            var properties = typeof(T).GetProperties();
            Dictionary<int, string> indexToPropertyDict = new Dictionary<int, string>();
            for (int i = 0; i < header.Length; i++)
            {
                foreach (var property in properties)
                {
                    if (property.Name == header[i])
                    {
                        indexToPropertyDict.Add(i, property.Name);
                    }
                }
            }

            List<T> resultObjects = new List<T>();
            for (int i = 0; i < lines.Count; i++)
            {
                try
                {
                    T newCreated = (T)Activator.CreateInstance(typeof(T));

                    for (int j = 0; j < indexToPropertyDict.Count; j++)
                    {
                        newCreated.GetType().GetProperty(indexToPropertyDict.GetValueOrDefault(j)).SetValue(newCreated, lines[i][j]);
                    }

                    resultObjects.Add(newCreated);
                }
                catch (Exception e)
                {
                    return Task.FromResult((false, new List<T>(), e.Message));
                }

            }

            return Task.FromResult((true, resultObjects, string.Empty));
        }
    }
}
