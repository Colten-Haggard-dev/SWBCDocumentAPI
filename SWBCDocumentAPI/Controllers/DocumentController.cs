using Amazon.S3.Model;
using Amazon.Textract.Model;
using Amazon.Textract;
using Microsoft.AspNetCore.Mvc;
using Amazon;
using System;
using SWBCDocumentAPI.Model;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Net.Http.Json;
using System.Text;

namespace SWBCDocumentAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController(ILogger<DocumentController> logger) : ControllerBase
{
    private readonly ILogger<DocumentController> _logger = logger;
    private readonly HttpClient _httpClient = new();

    [HttpPost("process")]
    public async Task<IActionResult> UploadFileAsync(UnprocessedDocument doc)
    {
        _httpClient.BaseAddress = new("https://localhost:7067/");

        MultipartFormDataContent form = [];
        form.Add(new StreamContent(doc.File.OpenReadStream()), "file", doc.File.FileName);

        HttpResponseMessage uploadResponse = await _httpClient.PostAsync("api/Textract/upload", form);

        if (uploadResponse.StatusCode == HttpStatusCode.OK)
        {
            return await FinishProcessingDocument(doc.Title, doc.File.FileName);
        }

        return BadRequest(uploadResponse.ReasonPhrase);
    }

    private async Task<IActionResult> FinishProcessingDocument(string title, string fileName)
    {
        string request = "api/Textract/detect?fileName=" + fileName;
        HttpResponseMessage detectResponse = await _httpClient.GetAsync(request);

        if (detectResponse.StatusCode != HttpStatusCode.OK)
        {
            return BadRequest(detectResponse.ReasonPhrase);
        }

        ProcessedDocument pdoc = new()
        {
            Title = title,
            RawText = await detectResponse.Content.ReadAsStringAsync()
        };

        return Ok(pdoc);
    }
}
