using TextractApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TextractService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/detect-text", async (TextractService textractService, string bucket, string filename) =>
{
    var jobId = await textractService.DetectText(bucket, filename);
    return Results.Ok(new { JobId = jobId });
})
.WithName("DetectText")
.WithOpenApi();


app.Run();
