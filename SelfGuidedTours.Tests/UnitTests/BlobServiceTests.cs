using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Moq;
using SelfGuidedTours.Core.Services.BlobStorage;

namespace SelfGuidedTours.Tests.UnitTests
{
    [TestFixture]
    public class BlobServiceTests
    {
        private Mock<BlobServiceClient> _blobServiceClientMock;
        private Mock<BlobContainerClient> _blobContainerClientMock;
        private Mock<BlobClient> _blobClientMock;
        private BlobService _blobService;

        [SetUp]
        public void Setup()
        {
            _blobServiceClientMock = new Mock<BlobServiceClient>();
            _blobContainerClientMock = new Mock<BlobContainerClient>();
            _blobClientMock = new Mock<BlobClient>();

            _blobServiceClientMock.Setup(b => b.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(_blobContainerClientMock.Object);

            _blobContainerClientMock.Setup(c => c.GetBlobClient(It.IsAny<string>()))
                .Returns(_blobClientMock.Object);

            _blobService = new BlobService(_blobServiceClientMock.Object);
        }

        [Test]
        public async Task DeleteFileAsync_ShouldDeleteFileIfExists()
        {
            _blobClientMock.Setup(b => b.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(true, null));

            await _blobService.DeleteFileAsync("testBlob", "testContainer");

            _blobClientMock.Verify(b => b.DeleteIfExistsAsync(It.IsAny<DeleteSnapshotsOption>(), It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void GetFileUrl_ShouldReturnBlobUrl()
        {
            var blobUri = new Uri("https://testaccount.blob.core.windows.net/testcontainer/testBlob");
            _blobClientMock.Setup(b => b.Uri).Returns(blobUri);

            var result = _blobService.GetFileUrl("testBlob", "testContainer");

            Assert.AreEqual(blobUri.ToString(), result);
        }

    }
}
