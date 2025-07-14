namespace DataProcessingService.Business.Tests;

[TestFixture]
public class StringProcessingServiceTests
{
    private StringProcessingService _service;

    [SetUp]
    public void SetUp()
    {
        _service = new StringProcessingService();
    }

    [Test]
    public void ProcessAsync_InputIsEmpty_ReturnsEmptyString()
    {
        // Arrange & Act
        var result = _service.Process(string.Empty);

        // Assert
        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ProcessAsync_SimpleString_ReturnsCorrectOutput()
    {
        // Arrange
        var input = "aabbc";
        var expected = $"a2b2c1/{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input))}";

        // Act
        var result = _service.Process(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ProcessAsync_HandlesSpecialCharactersCorrectly()
    {
        // Arrange
        var input = "a!a!";
        var expectedCounts = "!2a2";
        
        // Act
        var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input));
        var expected = $"{expectedCounts}/{base64}";

        var result = _service.Process(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void ProcessAsync_HandlesCaseSensitivity()
    {
        // Arrange
        var input = "AaA";
        
        // Act
        var expected = $"A2a1/{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input))}";
        var result = _service.Process(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}