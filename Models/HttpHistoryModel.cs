using System;

namespace plato.Models
{
    public class HttpHistoryModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public HttpRequestModel Request { get; set; } = new HttpRequestModel();
        public HttpResponseModel? Response { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string CollectionName { get; set; } = string.Empty;
    }
} 