using System.Net;

namespace Application.Common.Exceptions
{
    public class BussinessException : Exception
    {
        public string Code { get; set; }
        public int Status { get; set; }

        public BussinessException(string code, string message)
           : base(message)
        {
            Code = code;
            Status = (int)HttpStatusCode.BadRequest;
        }

        public static BussinessException FileSizeExceed() => new BussinessException("B01", "File size is exceed");

        public static BussinessException FileNoContent() => new BussinessException("B02", "File is not contained any data");

        public static BussinessException MultipartContentRequired() => new BussinessException("B03", "Multipart content type is required");

        public static BussinessException NoEncodingFound() => new BussinessException("B04", "No encoding information");

        public static BussinessException ValueCountExceed() => new BussinessException("B05", "Form key count limit of _defaultFormOptions.ValueCountLimit  is exceeded.");

        public static BussinessException InvalidSignatures() => new BussinessException("B06", "Invalid File Signatures");

        public static BussinessException FileNoContentDisposition() => new BussinessException("B07", "File is not contained content-disposition data");

        public static BussinessException InvalidExcelFileForm() => new BussinessException("B08", "Excel form does not match");

    }
}