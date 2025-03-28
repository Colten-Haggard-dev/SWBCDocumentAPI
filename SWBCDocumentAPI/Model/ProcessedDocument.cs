using Amazon.Textract.Model;

namespace SWBCDocumentAPI.Model;

/// <summary>
/// Represents a document that has been processed via OCR. The main text is stored as raw text.
/// </summary>
public class ProcessedDocument
{
    /// <summary>
    /// Title of the document.
    /// </summary>
    public string Title { get; set; } = "";
    /// <summary>
    /// Raw text body of the document.
    /// </summary>
    public string RawText { get; set; } = "";
    /// <summary>
    /// Time when the document was processed.
    /// </summary>
    public DateTime TimeStamp { get; set; } = DateTime.Now;
}