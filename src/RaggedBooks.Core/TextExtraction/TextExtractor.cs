using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.Outline;

namespace RaggedBooks.Core.TextExtraction;

public interface IConvertToBook
{
    Task<BookConversion.Book> Convert(string fileName);
}

public class BookConversion
{
    public record Page(
        string TextContent,
        int pagenumber);

    public record Chapter(
        string Title,
        int Level,
        int Pagenumber);

    public record Book(
        string Title,
        List<Page> Pages,
        List<Chapter> Chapters,
        string Authors,
        string Filename
    );
}


public class PdfToBookConverter : IConvertToBook
{
    public async Task<BookConversion.Book> Convert(string fileName)
    {
        using var pdfDocument = PdfDocument.Open(fileName);

        var pages = await GetContentAsync(pdfDocument);
        
        var chapters = await GetChapters(pdfDocument);

        var bookmarkTree = new BookmarkTree(chapters);
        
        var title = pdfDocument.Information.Title ?? string.Empty;
        var authors = pdfDocument.Information.Author ?? string.Empty;

        return new BookConversion.Book(title, pages, chapters, authors, Path.GetFileName(fileName));
    }

    private Task<List<BookConversion.Page>> GetContentAsync(PdfDocument pdfDocument)
    {
        var pages = new List<BookConversion.Page>();

        // get the title of the book
        foreach (var page in pdfDocument.GetPages().Where(x => x is not null))
        {
            var pageContent = ContentOrderTextExtractor.GetText(page) ?? string.Empty;
            pages.Add(new BookConversion.Page(pageContent, page.Number));
        }

        return Task.FromResult(pages);
    }

    private Task<List<BookConversion.Chapter>> GetChapters(PdfDocument pdfDocument)
    {
        var result = new List<BookConversion.Chapter>();

        if (pdfDocument.TryGetBookmarks(out var bookmarks))
        {
            var bookmarkNodes = bookmarks.GetNodes();
            foreach (BookmarkNode node in bookmarkNodes)
            {
                if (node is DocumentBookmarkNode docmark)
                {
                    result.Add(new BookConversion.Chapter(docmark.Title, docmark.Level, docmark.PageNumber));
                }
            }
        }

        return Task.FromResult(result);
    }
}
