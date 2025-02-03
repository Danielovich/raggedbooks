namespace RaggedBooks.Core.TextExtraction;

public record Book(
    string Title,
    List<Page> Pages,
    ChaperCollection ChapterPath,
    string Authors,
    string Filename
);