using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace plato.Models
{
    public class CollectionModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ObservableCollection<CollectionItemModel> Items { get; set; } = new ObservableCollection<CollectionItemModel>();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }

    public class CollectionItemModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public CollectionItemType Type { get; set; } = CollectionItemType.Request;
        public HttpRequestModel? Request { get; set; }
        public ObservableCollection<CollectionItemModel> Children { get; set; } = new ObservableCollection<CollectionItemModel>();
    }

    public enum CollectionItemType
    {
        Folder,
        Request
    }
} 