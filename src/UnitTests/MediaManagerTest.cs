using Core;
using Core.Abstraction;
using Core.Domain;
using LibVLCSharp.Shared;
using Moq;

namespace UnitTests;

public class MediaManagerTest
{
    private Mock<ILogger> _log;
    private Mock<LibVLC> _libVlc;
    private Mock<IMediaPlayerWrapper> _mediaPlayerWrapper;
    private Mock<IFileManager> _fileManager;

    public MediaManagerTest()
    {
        _log = new Mock<ILogger>();
        _libVlc = new Mock<LibVLC>();
        _mediaPlayerWrapper = new Mock<IMediaPlayerWrapper>();
        _fileManager = new Mock<IFileManager>();
    }

    private PlayManager SystemUnderTest => new (_log.Object, _mediaPlayerWrapper.Object, _fileManager.Object);


    [Theory]
    [InlineData("")]
    public async Task Play_ShouldThrowException(string path)
    {
        await Assert.ThrowsAnyAsync<ArgumentException>(() => SystemUnderTest.Play(path));
    }

    [Theory]
    [InlineData("//somepath/aaa/")]
    public async Task Play_PathNotFound_ShouldLogError(string path)
    {
        ResponseObject<int> responseObject = await SystemUnderTest.Play(path);

        _log.Verify(s => s.Error(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);

        Assert.Equal(ResponseEnum.Fail, responseObject.Response);
    }
    
    [Theory]
    [InlineData("//somepath/aaa/")]
    public async Task Play_PathFound_ShouldSucceed(string path)
    {
        _fileManager.Setup(s => s.Exists(It.IsAny<string>())).Returns(true);
        _mediaPlayerWrapper.Setup(s => s.Play(It.IsAny<string>()));      

        ResponseObject<int> responseObject = await SystemUnderTest.Play(path);
        Assert.Equal(ResponseEnum.Success, responseObject.Response);
    }
}