namespace RaggedBooks.Tests;

public class ChaperCollectionTests
{
    [Test]
    [CsvAutoData()]
    public void Returns_Correct_ChapterCollection(List<Chapter> chapters)
    {
        // Arrange
        var sut = new ChaperCollection(chapters);

        // Act
        var result = sut.ByPageNumber(47);

        // Assert 
        Assert.That(result.Equals("Preface > Chapter 1: Welcome to Software Construction > 2.2 How to Use Software Metaphors"));
        
    }

    [Test]
    public void Returns_Correct_ChapterCollection()
    {
        // Arrange
        var chapters = new List<Chapter>
        {
            new Chapter("Chapter 1", 0, 1),
            new Chapter("Foreword", 1, 3),
            new Chapter("Author", 1, 5),
            new Chapter("Acknowledgements", 2, 9)
        };

        var sut = new ChaperCollection(chapters);

        // Act Assert
        var path = sut.ByPageNumber(10);
        Assert.That(path.Equals("Chapter 1 > Author > Acknowledgements"));

        path = sut.ByPageNumber(5);
        Assert.That(path.Equals("Chapter 1 > Author"));
    }


    internal static class CsvDataLoader
    {
        public static List<Chapter> GetChaptersFromCsv()
        {
            var csvFilePath = string.Concat(
                Environment.CurrentDirectory,
                Path.DirectorySeparatorChar,
                "data",
                Path.DirectorySeparatorChar,
                "chapterpaths.csv");

            using var reader = new StreamReader(csvFilePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            return csv.GetRecords<Chapter>().ToList();
        }
    }

    internal class CsvAutoDataAttribute : AutoDataAttribute
    {
        public CsvAutoDataAttribute()
            : base(() => CreateFixtureWithCsvData())
        {
        }

        private static IFixture CreateFixtureWithCsvData()
        {
            var fixture = new Fixture();

            // Load CSV data
            var chapters = CsvDataLoader.GetChaptersFromCsv();

            // Customize the fixture to provide the CSV data
            fixture.Register(() => chapters);

            return fixture;
        }
    }
}