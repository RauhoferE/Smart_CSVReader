# Smart_CSVReader

## Usage

The following dll contains a simple CSV reader that can convert the read lines to an object.
The program is capable of reading CSV with and without a header.
The objects properties have to be primitiv or contain a ```.Parse``` Method.
```CSV
"Author"; "Title"; "Description"; "Year"
"Mike"; "My cool book"; "It's a very cool book."; "16.04.1998"
```

```CSHARP
    class TestClass
    {
        public string Author { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime Year { get; set; }
    }
```

To parse a CSV file use teh folllowing two methods:
> CSVReader.ParseCSVwithHeaderAsync<TestClass>(FILEPATH, DELIMITER)
> or
> CSVReader.ParseCSVAsync<TestClass>(FILEPATH,DELIMITER, new string[] { "Author", "Title", "Description", "Year" });

The properties will be automatically assigned to the different columns of the CSV file, if the name of the column and the name of the property match.
You can also use Attributes to assign theose values.
The priority for the name of the column is following.
1. Name attribute 
2. Index attribute
3. Property name

>You can also mix attributes.

The name attribute holds the column name of the csv file.
```CSHARP
    public class TestModelSubClassHeaderAttribute
    {
        [CSVHeaderNameAttribute("Author")]
        public string AuthorProp { get; set; }

        public string Title { get; set; }

        [CSVHeaderNameAttribute("Description")]
        public string DescriptionProp { get; set; }

        public DateTime Year { get; set; }
    }
```

2. The index attribute holds the position of the column in the csv file. (Count starts with 0)

```CSHARP
    class TestModelSubClassIndexAttribute
    {
        [CSVHeaderIndex(0)]
        public string AuthorProp { get; set; }

        public string Title { get; set; }

        [CSVHeaderIndex(2)]
        public string DescriptionProp { get; set; }

        public DateTime Year { get; set; }
    }
```