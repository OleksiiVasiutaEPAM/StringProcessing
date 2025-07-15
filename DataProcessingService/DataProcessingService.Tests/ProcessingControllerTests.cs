using DataProcessingService.Business.Contracts.Services;
using DataProcessingService.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DataProcessingService.Tests;

[TestFixture]
public class ProcessingControllerTests
{
    private Mock<IStringProcessingService> _processingServiceMock = null!;
    private ProcessingController _controller = null!;
    private MemoryStream _responseBodyStream = null!;

    [SetUp]
    public void SetUp()
    {
        _processingServiceMock = new Mock<IStringProcessingService>();
        _controller = new ProcessingController(_processingServiceMock.Object);

        var context = new DefaultHttpContext();
        _responseBodyStream = new MemoryStream();
        context.Response.Body = _responseBodyStream;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };
    }

    [Test]
    public async Task Stream_WritesMetadataAndCharacters()
    {
        // Arrange
        var input = "abc";
        var expectedResult = "a1b1c1/BASE64";
        _processingServiceMock.Setup(s => s.Process(input)).Returns(expectedResult);

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Act
        await _controller.Stream(input, token);

        // Assert
        _responseBodyStream.Seek(0, SeekOrigin.Begin);
        var output = await new StreamReader(_responseBodyStream).ReadToEndAsync(token);

        Assert.That(output, Does.Contain("event: metadata"));
        Assert.That(output, Does.Contain($"data: {expectedResult.Length}"));

        foreach (var ch in expectedResult)
        {
            Assert.That(output, Does.Contain($"data: {ch}"));
        }
    }

    [Test]
    public void Stream_ThrowsOnCancellation()
    {
        // Arrange
        var input = "abc";
        _processingServiceMock.Setup(s => s.Process(input)).Returns("abc/BASE");

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        Assert.ThrowsAsync<TaskCanceledException>(async () =>
        {
            await _controller.Stream(input, cts.Token);
        });
    }
}