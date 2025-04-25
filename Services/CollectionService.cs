using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using plato.Models;

namespace plato.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly string _collectionsDirectory;
        private ObservableCollection<CollectionModel> _collections;

        public CollectionService()
        {
            _collectionsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Plato",
                "Collections");
            
            // Ensure the directory exists
            if (!Directory.Exists(_collectionsDirectory))
            {
                Directory.CreateDirectory(_collectionsDirectory);
            }
            
            _collections = new ObservableCollection<CollectionModel>();
        }

        public async Task<ObservableCollection<CollectionModel>> GetCollectionsAsync()
        {
            if (_collections.Count == 0)
            {
                await LoadCollectionsAsync();
            }
            
            return _collections;
        }

        public async Task<CollectionModel> GetCollectionByIdAsync(Guid id)
        {
            if (_collections.Count == 0)
            {
                await LoadCollectionsAsync();
            }
            
            return _collections.FirstOrDefault(c => c.Id == id);
        }

        public async Task<bool> SaveCollectionAsync(CollectionModel collection)
        {
            collection.ModifiedAt = DateTime.Now;
            
            // If the collection doesn't exist, add it
            if (!_collections.Any(c => c.Id == collection.Id))
            {
                _collections.Add(collection);
            }
            else
            {
                // Otherwise update it
                var index = _collections.IndexOf(_collections.First(c => c.Id == collection.Id));
                _collections[index] = collection;
            }

            return await SaveCollectionsAsync();
        }

        public async Task<bool> DeleteCollectionAsync(Guid id)
        {
            var collection = _collections.FirstOrDefault(c => c.Id == id);
            if (collection != null)
            {
                _collections.Remove(collection);
                return await SaveCollectionsAsync();
            }
            
            return false;
        }

        public async Task<bool> SaveRequestToCollectionAsync(Guid collectionId, HttpRequestModel request, string name, string folderPath = "")
        {
            var collection = await GetCollectionByIdAsync(collectionId);
            if (collection == null)
            {
                return false;
            }

            // Create a new collection item for the request
            var requestItem = new CollectionItemModel
            {
                Name = name,
                Type = CollectionItemType.Request,
                Request = request
            };
            
            // If a folder path is specified, we need to add the request to that folder
            if (!string.IsNullOrEmpty(folderPath))
            {
                var folderNames = folderPath.Split('/').Where(f => !string.IsNullOrEmpty(f)).ToList();
                
                if (folderNames.Count > 0)
                {
                    // Navigate to the right folder
                    var currentItems = collection.Items;
                    foreach (var folderName in folderNames)
                    {
                        var folder = currentItems.FirstOrDefault(i => 
                            i.Type == CollectionItemType.Folder && i.Name == folderName);
                        
                        if (folder == null)
                        {
                            // Create the folder if it doesn't exist
                            folder = new CollectionItemModel
                            {
                                Name = folderName,
                                Type = CollectionItemType.Folder
                            };
                            currentItems.Add(folder);
                        }
                        
                        currentItems = folder.Children;
                    }
                    
                    // Add the request to the target folder
                    currentItems.Add(requestItem);
                }
                else
                {
                    // Add to the root level
                    collection.Items.Add(requestItem);
                }
            }
            else
            {
                // Add to the root level
                collection.Items.Add(requestItem);
            }
            
            return await SaveCollectionAsync(collection);
        }

        public async Task<CollectionItemModel> GetCollectionItemByIdAsync(Guid collectionId, Guid itemId)
        {
            var collection = await GetCollectionByIdAsync(collectionId);
            if (collection == null)
            {
                return null;
            }

            return FindItemById(collection.Items, itemId);
        }

        private CollectionItemModel FindItemById(ObservableCollection<CollectionItemModel> items, Guid itemId)
        {
            foreach (var item in items)
            {
                if (item.Id == itemId)
                {
                    return item;
                }

                if (item.Type == CollectionItemType.Folder)
                {
                    var childItem = FindItemById(item.Children, itemId);
                    if (childItem != null)
                    {
                        return childItem;
                    }
                }
            }

            return null;
        }

        private async Task LoadCollectionsAsync()
        {
            _collections.Clear();
            
            try
            {
                var files = Directory.GetFiles(_collectionsDirectory, "*.json");
                
                foreach (var file in files)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var collection = JsonSerializer.Deserialize<CollectionModel>(json);
                        
                        if (collection != null)
                        {
                            _collections.Add(collection);
                        }
                    }
                    catch (Exception)
                    {
                        // Skip files that can't be deserialized
                    }
                }
            }
            catch (Exception)
            {
                // Handle errors loading collections
            }
        }

        private async Task<bool> SaveCollectionsAsync()
        {
            try
            {
                foreach (var collection in _collections)
                {
                    var filePath = Path.Combine(_collectionsDirectory, $"{collection.Id}.json");
                    var json = JsonSerializer.Serialize(collection, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    
                    await File.WriteAllTextAsync(filePath, json);
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 