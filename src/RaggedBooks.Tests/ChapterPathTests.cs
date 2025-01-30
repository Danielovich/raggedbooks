using AutoFixture;
using AutoFixture.NUnit4;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using static RaggedBooks.Core.TextExtraction.BookConversion;

namespace RaggedBooks.Tests
{
    public static class CsvDataLoader
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

    public class CsvAutoDataAttribute : AutoDataAttribute
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


    public class ChapterPathTests
    {
        [Test]
        [CsvAutoData()]
        public void Page_Number_Returns_Chapter_Path(List<Chapter> chapters)
        {
            var sut = new ChapterPath(chapters);

            //var path = sut.ByPageNumber(123);
        }

        [Test]
        public void ByPageNumber_Returns_Correct_ChapterPath()
        {
            var chapters = new List<Chapter>
            {
                new Chapter("Chapter 1", 0, 1),
                new Chapter("Foreword", 1, 3),
                new Chapter("Author", 1, 5),
                new Chapter("Acknowledgements", 2, 9)
            };

            var sut = new ChapterPath(chapters);

            var path = sut.ByPageNumber(10);
            Assert.That(path.Equals("Chapter 1 > Author > Acknowledgements"));

            path = sut.ByPageNumber(5);
            Assert.That(path.Equals("Chapter 1 > Author"));
        }
    }

    public sealed class ChapterPath
    {
        private readonly IEnumerable<Chapter> chapters;

        public ChapterPath(IEnumerable<Chapter> chapters)
        {
            this.chapters = chapters;
        }

        internal string ByPageNumber(int pageNumber)
        {
            var path = new List<string>();

            Chapter? lastValidChapter = null;

            foreach (var chapter in chapters.OrderBy(c => c.Pagenumber))
            {
                // no reason to go beyond chapters with higher page numbers than we are looking for
                if (chapter.Pagenumber > pageNumber)
                    break;

                if (lastValidChapter == null || chapter.Level > lastValidChapter.Level)
                {
                    path.Add(chapter.Title);
                    lastValidChapter = chapter;
                }
                else if (chapter.Level == lastValidChapter.Level)
                {
                    // Replace the previous chapter at the same level
                    path[^1] = chapter.Title;
                    lastValidChapter = chapter;
                }
            }

            return string.Join(" > ", path);
        }
    }
}
