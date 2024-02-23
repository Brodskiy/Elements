namespace Core.DataStorage.Implementation
{
    internal abstract class StorageBase
    {
        public abstract void Save<T>(T data, string key);
        public abstract bool TryLoad<T>(string key, out T data);
        public abstract void Delete(string key);
    }
}