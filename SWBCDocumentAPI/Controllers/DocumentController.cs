using Amazon.S3.Model;
using Amazon.Textract.Model;
using Amazon.Textract;
using Microsoft.AspNetCore.Mvc;
using Amazon;
using System;

namespace SWBCDocumentAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpPost]
    public async Task<DetectDocumentTextResponse> UploadFileAsync(Model.Document doc)
    {
        DetectDocumentTextResponse detectResponse;
        using (var textractClient = new AmazonTextractClient(RegionEndpoint.USEast1))
        {
            Stream rstream = doc.File.OpenReadStream();
            var bytes = new byte[doc.File.Length];
            rstream.ReadExactly(bytes, 0, bytes.Length);
            rstream.Close();

            Console.WriteLine("Detect Document Text");
            detectResponse = await textractClient.DetectDocumentTextAsync(new DetectDocumentTextRequest
            {
                Document = new Amazon.Textract.Model.Document
                {
                    Bytes = new MemoryStream(bytes)
                }
            });

            foreach (var block in detectResponse.Blocks)
            {
                Console.WriteLine($"Type {block.BlockType}, Text: {block.Text}");
            }
        }

        return detectResponse;
    }
}
