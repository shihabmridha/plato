using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using plato.Models;

namespace plato.Services
{
    public interface ICollectionService
    {
        Task<ObservableCollection<CollectionModel>> GetCollectionsAsync();
        Task<CollectionModel> GetCollectionByIdAsync(Guid id);
        Task<bool> SaveCollectionAsync(CollectionModel collection);
        Task<bool> DeleteCollectionAsync(Guid id);
        Task<bool> SaveRequestToCollectionAsync(Guid collectionId, HttpRequestModel request, string name, string folderPath = "");
        Task<CollectionItemModel> GetCollectionItemByIdAsync(Guid collectionId, Guid itemId);
    }
} 