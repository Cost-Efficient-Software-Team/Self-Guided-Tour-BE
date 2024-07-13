using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace SelfGuidedTours.Tests.UnitTests
{
    public class FormFileMock : IFormFile
    {
        private readonly Stream stream;
        private readonly string fileName;
        private readonly string contentType;
        private readonly long length;

        public FormFileMock(Stream stream, string fileName, string contentType)
        {
            this.stream = stream;
            this.fileName = fileName;
            this.contentType = contentType;
            this.length = stream.Length;
        }

        public string ContentType => contentType;
        public string ContentDisposition => $"form-data; name=\"{fileName}\"; filename=\"{fileName}\"";
        public IHeaderDictionary Headers => new HeaderDictionary();
        public long Length => length;
        public string Name => fileName;
        public string FileName => fileName;

        public void CopyTo(Stream target)
        {
            stream.CopyTo(target);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return stream.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            return stream;
        }
    }

    public class HeaderDictionary : Dictionary<string, StringValues>, IHeaderDictionary
    {
        public long? ContentLength
        {
            get
            {
                if (TryGetValue("Content-Length", out var values) &&
                    long.TryParse(values.ToString(), out var length))
                {
                    return length;
                }

                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    this["Content-Length"] = value.Value.ToString();
                }
                else
                {
                    Remove("Content-Length");
                }
            }
        }
    }
}
