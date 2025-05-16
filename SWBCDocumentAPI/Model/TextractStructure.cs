using static System.Net.Mime.MediaTypeNames;

namespace SWBCDocumentAPI.Model;

public class TextractPage
{

}

public class TextractDocumentObject(string title, string id, string text)
{
    public string Title { get; private set; } = title;
    public string Id { get; private set; } = id;
    public string Text { get; private set; } = text;
}

public class TextractDocument
{
    

    protected List<TextractDocumentObject> _children = [];
}

public class TextractTable(string title, string id, string text) : TextractDocumentObject(title, id, text)
{
    
}
