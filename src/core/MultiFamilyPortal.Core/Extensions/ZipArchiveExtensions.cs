using System.IO.Compression;

namespace MultiFamilyPortal.Extensions
{
    public static class ZipArchiveExtensions
    {
        public static void AddFile(this ZipArchive archive, string fileName, byte[] data)
        {
            var zipArchiveEntry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
            using var zipStream = zipArchiveEntry.Open();
            zipStream.Write(data, 0, data.Length);
        }
    }
}
