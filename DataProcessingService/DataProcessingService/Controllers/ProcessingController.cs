using DataProcessingService.Business.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataProcessingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessingController(IProcessingService processingService) : ControllerBase
{
    [HttpGet("stream")]
    public async Task Stream([FromQuery] string input, CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";

        var result = processingService.ProcessAsync(input);
    
        await Response.WriteAsync($"event: metadata\ndata: {result.Length}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);

        foreach (var ch in result)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(Random.Shared.Next(1000, 5000), cancellationToken);
            await Response.WriteAsync($"data: {ch}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}