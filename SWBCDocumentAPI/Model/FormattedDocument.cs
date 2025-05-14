using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.AspNetCore.Html;
using System.Text;

namespace SWBCDocumentAPI.Model;

/// <summary>
/// A static class that stores constants for what mode the text should be converted to
/// </summary>
public static class TextFormats
{
    public const string RAW = "RAW";
    public const string PRETTY = "PRETTY";
    public const string HTML = "HTML";
}


/// <summary>
/// An abstract class for different types of document formatters to inherit from.
/// </summary>
/// <param name="format">The format that a formatter should format a document to.</param>
public abstract class DocumentFormatter(string format)
{
    protected readonly string _format = format;
    public string EncodedText { get; protected set; } = "";

    protected abstract void ToRaw(List<Block> blocks);
    protected abstract void ToPretty(List<Block> blocks);
    protected abstract void ToHTML(List<Block> blocks);

    public void TextractToFormat(List<Block> blocks)
    {
        switch (_format)
        {
            case TextFormats.RAW:
                ToRaw(blocks);
                break;
            case TextFormats.PRETTY:
                ToPretty(blocks);
                break;
            case TextFormats.HTML:
                ToHTML(blocks);
                break;
            default:
                Console.Error.WriteLine("ERROR: Invalid format specifier.");
                break;
        }
    }
}

/// <summary>
/// Formats a document that had the detect form of extraction used on it.
/// </summary>
/// <param name="format">The format the detected document should be formatted to.</param>
public class DocumentFormatterDetected(string format) : DocumentFormatter(format)
{
    protected override void ToRaw(List<Block> blocks)
    {
        StringBuilder builder = new();

        foreach (var block in blocks)
        {
            builder.Append(block.Text);
        }

        EncodedText = builder.ToString();
    }

    protected override void ToPretty(List<Block> blocks)
    {
        StringBuilder builder = new();

        foreach (var block in blocks)
        {
            if (block.BlockType == BlockType.LINE)
            {
                builder.AppendLine(block.Text);
            }
        }

        EncodedText = builder.ToString();
    }

    protected override void ToHTML(List<Block> blocks)
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

        EncodedText = builder.ToString();
    }
}

public class DocumentFormatterAnalyzed(string format) : DocumentFormatter(format)
{
    protected override void ToRaw(List<Block> blocks)
    {
        StringBuilder builder = new();

        foreach (var block in blocks)
        {
            builder.Append(block.Text);
        }

        EncodedText = builder.ToString();
    }

    protected override void ToPretty(List<Block> blocks)
    {
        StringBuilder builder = new();

        foreach (var block in blocks)
        {
            builder.AppendLine($"{block.Text}:{block.BlockType}:{block.TextType}");
        }

        EncodedText = builder.ToString();
    }

    protected override void ToHTML(List<Block> blocks)
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
            else if (block.BlockType == BlockType.TABLE)
            {
                builder.Append($"<table></table>");
            }
        }

        EncodedText = builder.ToString();
    }
}
