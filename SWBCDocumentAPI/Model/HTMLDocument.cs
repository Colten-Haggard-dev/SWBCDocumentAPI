using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.AspNetCore.Html;
using System.Text;
using System.Text.Encodings.Web;

namespace SWBCDocumentAPI.Model;

public abstract class HTMLDocument
{
    public string HTMLEncodedText { get; protected set; } = "";

    public abstract void TextractToHTML(List<Block> doc);
}

public class HTMLDocumentDetected : HTMLDocument
{
    public override void TextractToHTML(List<Block> blocks)
    {
        StringBuilder builder = new();

        foreach (var block in blocks)
        {
            if (block.BlockType == BlockType.PAGE)
            {
                builder.AppendLine("<div>");
            }
            else if (block.BlockType == BlockType.LINE)
            {
                builder.AppendLine($"<p>{block.Text}</p><br>");
            }
        }

        builder.Append("</div>");

        HTMLEncodedText = builder.ToString();
    }
}

public class HTMLDocumentAnalyzed : HTMLDocument
{
    public override void TextractToHTML(List<Block> doc)
    {
        StringBuilder builder = new();

        foreach (var block in doc)
        {
            builder.Append(block.BlockType);
            builder.Append(' ');
        }

        HTMLEncodedText = builder.ToString();
    }
}
