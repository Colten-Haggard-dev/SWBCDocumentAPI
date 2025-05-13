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
using System.Diagnostics.Contracts;

namespace SWBCDocumentAPI.Controllers;

public static class ProcessMethods
{
    public const string ANALYZE = "ANALYZE";
    public const string DETECT = "DETECT";
}

/// <summary>
/// <c>DocumentController</c> is the controller that allows HTTP requests to be handled for Documents (<seealso cref="UnprocessedDocument"/> and <seealso cref="ProcessedDocument"/>).
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly ILogger<DocumentController> _logger;
    private readonly HttpClient _httpClient;

    public DocumentController(ILogger<DocumentController> logger)
    {
        _logger = logger;
        _httpClient = new()
        {
            BaseAddress = new("https://localhost:7067/")
        };
    }

    /// <summary>
    /// Uses the "process" route as a post request to allow a client to upload an <seealso cref="UnprocessedDocument"/> and then have it processed by the OCR API.
    /// </summary>
    /// <param name="doc">The <seealso cref="UnprocessedDocument"/> the client uploads.</param>
    /// <returns>If file upload is successful Ok is returned with the jobId of the detection job, elsewise a BadRequest is returned.</returns>
    [HttpPost("process")]
    public async Task<IActionResult> ProcessDocumentAsync(UnprocessedDocument doc, string method)
    {
        HttpResponseMessage upload = UploadDocument(doc);

        if (!upload.IsSuccessStatusCode)
            return BadRequest(upload.Content);

        HttpResponseMessage detect;
        if (method == ProcessMethods.DETECT)
            detect = StartDetection(doc);
        else if (method == ProcessMethods.ANALYZE)
            detect = StartAnalysis(doc);
        else
            return BadRequest($"{method} is not a recognized processing method.");

        if (!detect.IsSuccessStatusCode)
            return BadRequest(upload.Content);

        string jobId = await detect.Content.ReadAsStringAsync();

        return Ok(jobId);
    }

    /// <summary>
    /// Uses the "getProcessedDocument" route as a get request to allow a client to get a processed document
    /// from the TextractOCR API
    /// </summary>
    /// <param name="jobId">The ID of a job in TextractOCR, this is used to retrieved a processed document</param>
    /// <returns>A processed document formatted in HTML</returns>
    [HttpGet("getProcessedDocument")]
    public async Task<IActionResult> GetProcessedDocument(string jobId, string method, string format)
    {
        HttpResponseMessage check;
        DocumentFormatter doc;

        if (method == ProcessMethods.DETECT)
        {
            check = CheckDetection(jobId);
            doc = new DocumentFormatterDetected(format);
        }
        else if (method == ProcessMethods.ANALYZE)
        {
            check = CheckAnalysis(jobId);
            doc = new DocumentFormatterAnalyzed(format);
        }
        else
            return BadRequest($"{method} is not a recognized processing method.");

        if (!check.IsSuccessStatusCode)
            return BadRequest(check.Content);

        List<Block>? blocks = (await check.Content.ReadFromJsonAsync<GetDocumentTextDetectionResponse>())?.Blocks;
        if (blocks == null)
            return BadRequest("No blocks found");

        doc.TextractToFormat(blocks);

        return Ok(doc);
    }

    /// <summary>
    /// A helper function to upload documents to the Textract API
    /// </summary>
    /// <param name="doc">The document and its info to upload</param>
    /// <returns>The upload response</returns>
    private HttpResponseMessage UploadDocument(UnprocessedDocument doc)
    {
        MultipartFormDataContent form = [];
        form.Add(new StreamContent(doc.File.OpenReadStream()), "file", doc.File.FileName);

        Task<HttpResponseMessage> uploadResponse = _httpClient.PostAsync("api/Textract/upload", form);
        uploadResponse.Wait();

        return uploadResponse.Result;
    }

    /// <summary>
    /// A helper function to begin document detection on the TextractOCR
    /// </summary>
    /// <param name="doc">The document that is to be processed</param>
    /// <returns>The begin detection response</returns>
    private HttpResponseMessage StartDetection(UnprocessedDocument doc)
    {
        string request = "api/Textract/beginDetect?fileName=" + doc.File.FileName;
        Task<HttpResponseMessage> detectResponse =  _httpClient.GetAsync(request);
        detectResponse.Wait();

        return detectResponse.Result;
    }

    /// <summary>
    /// A helper function to begin document analysis on the TextractOCR
    /// </summary>
    /// <param name="doc">The document that is to be processed</param>
    /// <returns>The begin analysis response</returns>
    private HttpResponseMessage StartAnalysis(UnprocessedDocument doc)
    {
        string request = "api/Textract/beginAnalysis?fileName=" + doc.File.FileName;
        Task<HttpResponseMessage> detectResponse = _httpClient.GetAsync(request);
        detectResponse.Wait();

        return detectResponse.Result;
    }

    /// <summary>
    /// A helper function to check if a document has finished being processed(detection) on the TextractOCR API
    /// </summary>
    /// <param name="jobId">The ID of the job the caller wants to check</param>
    /// <returns>The check response</returns>
    private HttpResponseMessage CheckDetection(string jobId)
    {
        string request = "api/Textract/checkDetect?jobId=" + jobId;
        Task<HttpResponseMessage> checkResponse = _httpClient.GetAsync(request);
        checkResponse.Wait();

        return checkResponse.Result;
    }

    /// <summary>
    /// A helper function to check if a document has finished being processed(analysis) on the TextractOCR API
    /// </summary>
    /// <param name="jobId">The ID of the job the caller wants to check</param>
    /// <returns>The check response</returns>
    private HttpResponseMessage CheckAnalysis(string jobId)
    {
        string request = "api/Textract/checkAnalysis?jobId=" + jobId;
        Task<HttpResponseMessage> checkResponse = _httpClient.GetAsync(request);
        checkResponse.Wait();

        return checkResponse.Result;
    }
}
