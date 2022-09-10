using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP3
{
    public class XP3ArchiveWriter
    {
        private static readonly byte[] HeaderMagic = { 0x58, 0x50, 0x33, 0x0D, 0x0A, 0x20, 0x0A, 0x1A, 0x8B, 0x67, 0x01 };

        public static void Write(string folderPath, string xp3FilePath)
        {
            if (!folderPath.EndsWith("\\") && !folderPath.EndsWith("/"))
                folderPath += "\\";

            Console.WriteLine($"Packing {xp3FilePath}");
            using Stream xp3Stream = File.Open(xp3FilePath, FileMode.Create, FileAccess.Write);
            using BinaryWriter xp3Writer = new(xp3Stream);
            xp3Writer.Write(HeaderMagic);
            xp3Writer.Write(0L);        // Index offset

            Xp3IndexBuilder index = new();

            foreach (string filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(filePath);
                bool compressed = extension != ".mpg";

                long offset = xp3Stream.Position;
                AppendFile(xp3Stream, filePath, compressed, out long originalSize, out long compressedSize);

                string relativeFilePath = filePath[folderPath.Length..];
                index.Add(relativeFilePath, offset, originalSize, compressedSize, compressed);
            }

            long indexOffset = xp3Stream.Length;
            xp3Writer.Write(index.Build());

            xp3Stream.Seek(HeaderMagic.Length, SeekOrigin.Begin);
            xp3Writer.Write(indexOffset);
        }

        private static void AppendFile(Stream xp3Stream, string filePath, bool compressed, out long originalSize, out long compressedSize)
        {
            using Stream fileStream = File.OpenRead(filePath);
            originalSize = fileStream.Length;

            long startPos = xp3Stream.Position;
            if (compressed)
            {
                using ZlibStream compressionStream = new(xp3Stream);
                fileStream.CopyTo(compressionStream);
            }
            else
            {
                fileStream.CopyTo(xp3Stream);
            }

            compressedSize = xp3Stream.Position - startPos;
        }
    }
}
