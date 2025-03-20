using Amazon.Textract.Model;

namespace SWBCDocumentAPI.Model;

public class ProcessedDocument
{
    public string Title { get; set; } = "";
    public string RawText { get; set; } = "";
    public DateTime TmeStamp { get; set; } = DateTime.Now;
}