using System;
using System.Collections.Generic;
using System.Net;

namespace plato.Models
{
    public class HttpResponseModel
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ReasonPhrase { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Body { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long ElapsedMilliseconds { get; set; }
        public DateTime ResponseTime { get; set; } = DateTime.Now;
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
} 