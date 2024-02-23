using System;
using System.Collections.Generic;
using Core.DataStorage.Data;
using Core.DataStorage.Declaration;
using Core.DataStorage.Implementation.Storages;
using Core.Foundation.Declaration;
using Cysharp.Threading.Tasks;
using Debug = Core.DataStorage.Log.Logger;

namespace Core.DataStorage.Implementation
{
    public class DataStorage : IDataStorage, IInitialization, IDisposable
    {
        private const StorageType DefaultStorageType = StorageType.PlayerPrefs;

        private Dictionary<StorageType, StorageBase> _storages;

        public async UniTask InitializeAsync()
        {
            _storages = new Dictionary<StorageType, StorageBase>
            {
                [StorageType.PlayerPrefs] = new PlayerPrefsStorage(),
            };

            await UniTask.CompletedTask;
        }

        public void Save<T>(T data, string key)
        {
            Save(data, key, DefaultStorageType);
        }

        public void Save<T>(T data, string key, StorageType storageType)
        {
            if (!TryGetStorageByType(storageType, out var storage))
            {
                return;
            }
            
            storage.Save(data, key);
        }

        public bool TryLoad<T>(string key, out T data)
        {
            return TryLoad(key, out data, DefaultStorageType);
        }

        public bool TryLoad<T>(string key, out T data, StorageType storageType)
        {
            data = default;
            return TryGetStorageByType(storageType, out var storage) && storage.TryLoad(key, out data);
        }

        public void Delete(string key)
        {
            Delete(key, DefaultStorageType);
        }

        public void Delete(string key, StorageType storageType)
        {
            if (!TryGetStorageByType(storageType, out var storage))
            {
                return;
            }

            storage.Delete(key);
        }

        public void Dispose()
        {
            _storages = null;
        }

        private bool TryGetStorageByType(StorageType storageType, out StorageBase storage)
        {
            if (_storages == null)
            {
                Debug.LogError("Storages equal null.");
                storage = null;
                return false;
            }
            
            if (_storages.TryGetValue(storageType, out var storageBase))
            {
                storage = storageBase;
                return true;
            }

            Debug.LogError($"Storage by type - {storageType} not found.");
            storage = null;
            return false;
        }
    }
}