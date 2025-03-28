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

/// <summary>
/// <c>DocumentController</c> is the controller that allows HTTP requests to be handled for Documents (<seealso cref="UnprocessedDocument"/> and <seealso cref="ProcessedDocument"/>).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class DocumentController(ILogger<DocumentController> logger) : ControllerBase
{
    private readonly ILogger<DocumentController> _logger = logger;
    private readonly HttpClient _httpClient = new();

    private readonly Dictionary<Guid, ProcessedDocument> _documentJobs = [];

    /// <summary>
    /// Uses the "process" route as a post request to allow a client to upload an <seealso cref="UnprocessedDocument"/> and then have it processed by the OCR API.
    /// </summary>
    /// <param name="doc">The <seealso cref="UnprocessedDocument"/> the client uploads.</param>
    /// <returns>If file upload is successful, a <seealso cref="Guid"/> (jobID) is returned with Ok, elsewise a BadRequest is returned.</returns>
    [HttpPost("process")]
    public async Task<IActionResult> ProcessDocumentAsync(UnprocessedDocument doc)
    {
        _httpClient.BaseAddress = new("https://localhost:7067/");

        MultipartFormDataContent form = [];
        form.Add(new StreamContent(doc.File.OpenReadStream()), "file", doc.File.FileName);

        HttpResponseMessage uploadResponse = await _httpClient.PostAsync("api/Textract/upload", form);

        if (uploadResponse.StatusCode == HttpStatusCode.OK)
        {
            Guid jobId = Guid.NewGuid();
            FinishProcessingDocument(jobId, doc.Title, doc.File.FileName);
            return Ok(jobId);
        }

        return BadRequest(uploadResponse.ReasonPhrase);
    }

    /// <summary>
    /// Uses the "getJob" route as a get request to allow a client to get their document back as a <seealso cref="ProcessedDocument"/>.
    /// It is expected that a client will call this regularly as it waits for their document to complete processing.
    /// </summary>
    /// <param name="jobId">The <seealso cref="Guid"/> of a document process job.</param>
    /// <returns>If successful, returns a <seealso cref="ProcessedDocument"/> containing the information of their processed document, elsewise a <seealso cref="null"/> is returned.</returns>
    [HttpGet("getJob")]
    public ProcessedDocument? GetJob(Guid jobId)
    {
        if (_documentJobs.TryGetValue(jobId, out var job))
        {
            return job;
        }

        return null;
    }

    /// <summary>
    /// A helper function that calls the OCR API to process the uploaded document.
    /// </summary>
    /// <param name="jobId">The <seealso cref="Guid"/> to be used as a job ID in the <c>_documentJobs</c>.</param>
    /// <param name="title">The title of the document.</param>
    /// <param name="fileName">The file name of the document.</param>
    private async void FinishProcessingDocument(Guid jobId, string title, string fileName)
    {
        string request = "api/Textract/detect?fileName=" + fileName;
        HttpResponseMessage detectResponse = await _httpClient.GetAsync(request);

        if (detectResponse.StatusCode != HttpStatusCode.OK)
        {
            return;
        }

        ProcessedDocument pdoc = new()
        {
            Title = title,
            RawText = await detectResponse.Content.ReadAsStringAsync()
        };

        _documentJobs[jobId] = pdoc;
    }
}
