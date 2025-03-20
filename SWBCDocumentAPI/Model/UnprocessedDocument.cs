namespace SWBCDocumentAPI.Model;

public class UnprocessedDocument
{
    public string Title { get; set; } = "default_doc";
    public required IFormFile File { get; set; }
    public DateTime TimeStamp { get; set; } = DateTime.Now;
}
