namespace SWBCDocumentAPI.Model;

public class Document
{
    public string Title { get; set; } = "default_doc";
    public DateTime TimeStamp { get; set; }
    public IFormFile File { get; set; }
}
