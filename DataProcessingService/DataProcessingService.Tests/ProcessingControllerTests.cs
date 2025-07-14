using DataProcessingService.Business.Contracts.Services;
using DataProcessingService.Controllers;
using DataProcessingService.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DataProcessingService.Tests;

[TestFixture]
public class ProcessingControllerTests
{
    private Mock<IStringProcessingService> _mockService = null!;
    private ProcessingController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IStringProcessingService>();
        _controller = new ProcessingController(_mockService.Object);
        
        var context = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };
    }

    [Test]
    public async Task Stream_ShouldStreamMetadataAndCharacters()
    {
        // Arrange
        const string input = "aab";
        const string processedResult = "a2b1/processedBase64==";

        _mockService.Setup(s => s.Process(input)).Returns(processedResult);

        // Act
        var result = _controller.Stream(input, CancellationToken.None);

        var streamedLines = new List<string>();
        await foreach (var line in result)
        {
            streamedLines.Add(line);
        }

        // Assert
        Assert.That(streamedLines, Is.Not.Empty);
        Assert.That(streamedLines.First(), Is.EqualTo($"event: metadata\ndata: {processedResult.Length}\n\n"));
        Assert.That(streamedLines.Count, Is.EqualTo(1 + processedResult.Length));

        var expectedCharacters = processedResult.ToCharArray();
        for (var i = 0; i < expectedCharacters.Length; i++)
        {
            Assert.That(streamedLines[i + 1], Is.EqualTo($"data: {expectedCharacters[i]}\n\n"));
        }
    }
    
    [Test]
    public void Stream_InvalidModel_ThrowsValidationFailedException()
    {
        // Arrange
        _controller.ModelState.AddModelError("input", "The input field is required.");

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationFailedException>(async () =>
        {
            await foreach (var _ in _controller.Stream("", CancellationToken.None)) { }
        });

        Assert.That(ex!.Errors, Contains.Key("input"));
    }
}