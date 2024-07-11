using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Models.ErrorResponse
{
    public class ErrorDetails
    {
        public Guid ErrorId { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public IDictionary<string, string> Errors { get; set; } = new Dictionary<string, string>();

        //Override the ToString method to return the object as a JSON string
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
