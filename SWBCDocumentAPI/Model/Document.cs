namespace SWBCDocumentAPI.Model;

public class Document
{
    public DateTime Date { get; set; }
    public IFormFile? DocumentFile { get; set; }
}
