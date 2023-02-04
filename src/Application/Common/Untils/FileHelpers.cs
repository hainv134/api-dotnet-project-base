using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Application.Common.Untils
{
    public class FileHelpers
    {
        private static readonly Dictionary<string, List<byte[]>> _fileSignature = new Dictionary<string, List<byte[]>>
    {
        // List of file signatures
        // https://en.wikipedia.org/wiki/List_of_file_signatures
        { ".pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D } } },
        { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
        { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
        { ".jpeg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
            }
        },
        { ".jpg", new List<byte[]>
            {
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
            }
        },
        {
            ".xlsx", new List<byte[]>{
                new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                new byte[] { 0x50, 0x4B, 0x04, 0x06 },
                new byte[] { 0x50, 0x4B, 0x07, 0x08 },
            }
        },
    };

        public static async Task<byte[]> ProcessFormFile<T>(IFormFile formFile,
            string[] permittedExtensions,
            long sizeLimit)
        {
            // Check the file length. This check doesn't catch files that only have a BOM as their content
            if (formFile.Length == 0)
            {
                throw BussinessException.FileNoContent();
            }

            if (formFile.Length > sizeLimit)
            {
                throw BussinessException.FileSizeExceed();
            }

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);

                    if (memoryStream.Length == 0)
                    {
                        throw BussinessException.FileNoContent();
                    }

                    if (!IsValidFileExtensionAndSignature(formFile.FileName, memoryStream, permittedExtensions))
                    {
                        throw BussinessException.InvalidSignatures();
                    }
                    else
                    {
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        public static async Task<byte[]> ProcessStreamedFile(
            MultipartSection section,
            ContentDispositionHeaderValue contentDisposition,
            string[] permittedExtensions,
            long sizeLimit)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await section.Body.CopyToAsync(memoryStream);

                    // Check if the file is empty or exceeds the size limit
                    if (memoryStream.Length == 0)
                    {
                        throw BussinessException.FileNoContent();
                    }
                    else if (memoryStream.Length > sizeLimit)
                    {
                        throw BussinessException.FileSizeExceed();
                    }
                    else if (!IsValidFileExtensionAndSignature(
                                 contentDisposition.FileName.Value ?? "",
                                 memoryStream,
                                 permittedExtensions))
                    {
                        throw BussinessException.InvalidSignatures();
                    }
                    else
                    {
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /**
         * <summary>Checking file extension is valid or not</summary>
         */

        public static bool IsValidFileExtensionAndSignature(
            string fileName,
            Stream data,
            string[] permittedExtensions)
        {
            // If data is null or empty
            if (string.IsNullOrEmpty(fileName)
                || data == null
                || data.Length == 0)
            {
                return false;
            }

            // Convert file extension to lower case
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            // File extension must not null, empty or not-permitted
            if (string.IsNullOrEmpty(extension)
                || !permittedExtensions.Contains(extension))
            {
                return false;
            }

            data.Position = 0;

            using var reader = new BinaryReader(data);
            // File signature check
            var signature = _fileSignature[extension];
            var headerBytes = reader.ReadBytes(signature.Max(m => m.Length));

            return signature.Any(s => headerBytes.Take(s.Length).SequenceEqual(s));
        }
    }
}