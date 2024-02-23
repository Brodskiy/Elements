using Core.DataStorage.Data;

namespace Core.DataStorage.Declaration
{
    public interface IDataStorage
    {
        void Save<T>(T data, string key);
        void Save<T>(T data, string key, StorageType storageType);
        bool TryLoad<T>(string key, out T data);
        bool TryLoad<T>(string key, out T data, StorageType storageType);
        void Delete(string key);
        void Delete(string key, StorageType storageType);
    }
}