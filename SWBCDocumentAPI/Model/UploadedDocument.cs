namespace SWBCDocumentAPI.Model;

public class UploadedDocument
{
    public required IFormFile File { get; set; }
    public string Method { get; set; } = "NONE";
}
