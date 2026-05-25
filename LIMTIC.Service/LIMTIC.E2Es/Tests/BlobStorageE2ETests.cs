using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.MailFixture;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class BlobStorageE2ETests : BaseE2ETests
    {
        public BlobStorageE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        [Fact]
        public async Task BlobStorageService_UploadsPdfE2ETest()
        {
            var pdfPath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "LIMTIC_Backend_Blueprint.pdf");
            Assert.True(File.Exists(pdfPath), $"Test file was not found: {pdfPath}");

            var pdfBytes = await File.ReadAllBytesAsync(pdfPath);
            await using var pdfStream = new MemoryStream(pdfBytes);

            var uploaded = await BlobStorageService.UploadStreamAsync(
                pdfStream,
                "test-container",
                $"test-path/{Guid.NewGuid():N}.pdf",
                overwrite: true);

            Assert.True(uploaded);
        }
    }
}
