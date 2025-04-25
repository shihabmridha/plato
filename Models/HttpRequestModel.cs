using System;
using System.Collections.Generic;
using System.Net.Http;

namespace plato.Models
{
    public class HttpRequestModel
    {
        public string Url { get; set; } = string.Empty;
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>();
        public string Body { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/json";
        public AuthenticationType AuthType { get; set; } = AuthenticationType.None;
        public Dictionary<string, string> AuthParameters { get; set; } = new Dictionary<string, string>();
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastExecutedAt { get; set; }
    }
    
    public enum AuthenticationType
    {
        None,
        Basic,
        Bearer,
        ApiKey,
        OAuth2
    }
} 