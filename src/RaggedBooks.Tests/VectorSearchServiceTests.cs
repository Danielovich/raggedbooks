namespace RaggedBooks.Tests;

public class VectorSearchServiceTests
{
    [Test]
    public async Task TestGetBooks()
    {
        var svc = new QDrantApiClient(
            new RaggedBookConfig() { QdrantUrl = new Uri("http://localhost:6333") }
        );

        var books = await svc.GetBooks();
        Assert.That(books.Keys.Count, Is.GreaterThanOrEqualTo(1));
    }
}
