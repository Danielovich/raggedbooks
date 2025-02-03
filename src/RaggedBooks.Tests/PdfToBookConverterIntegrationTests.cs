namespace RaggedBooks.Tests;

public class PdfToBookConverterIntegrationTests
{
    [Test]
    public async Task Pdf_Extraction_Succeeds()
    {
        // Arrange
        var file = string.Concat(
            Environment.CurrentDirectory,
            Path.DirectorySeparatorChar,
            "data",
            Path.DirectorySeparatorChar,
            "warandpeace.pdf"); //public domain book

        var sut = new PdfToBookConverter();

        // Act
        var book = await sut.Convert(file);

        // Assert
        Assert.That(book, Is.Not.Null);
        Assert.That(book.Title, Is.EqualTo("War and Peace"));
        Assert.That(book.Pages.Count, Is.GreaterThanOrEqualTo(1000));
    }
}
