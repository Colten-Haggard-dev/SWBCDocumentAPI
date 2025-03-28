namespace SWBCDocumentAPI.Model;

/// <summary>
/// Represents a document that has yet to be processed by OCR.
/// </summary>
public class UnprocessedDocument
{
    /// <summary>
    /// Title of the document.
    /// </summary>
    public string Title { get; set; } = "";
    /// <summary>
    /// Actual file sent by the client.
    /// </summary>
    public required IFormFile File { get; set; }
    /// <summary>
    /// Time when the document was uploaded.
    /// </summary>
    public DateTime TimeStamp { get; set; } = DateTime.Now;
}
