using Amazon.Textract;
using Amazon.Textract.Model;
using Amazon;

namespace TextractApi.Services
{
    public class TextractService
    {
        private readonly AmazonTextractClient _client;

        public TextractService()
        {
            // Specify your region here
            var region = RegionEndpoint.USEast2;  // Replace with the region where your Textract service is available
            _client = new AmazonTextractClient(region);
        }

        public async Task<string> DetectText(string bucketName, string fileName)
        {
            var request = new StartDocumentTextDetectionRequest
            {
                DocumentLocation = new DocumentLocation
                {
                    S3Object = new S3Object
                    {
                        Bucket = bucketName,
                        Name = fileName
                    }
                }
            };

            var response = await _client.StartDocumentTextDetectionAsync(request);
            return response.JobId;
        }
    }
}
