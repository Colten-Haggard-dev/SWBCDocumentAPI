using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWBCDocumentAPI.Model;

namespace SWBCDocumentAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpPost]
    public string UploadFile(IFormFile file)
    {
        return file.FileName;
    }
}
