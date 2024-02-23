using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Core.DataStorage.Implementation.Storages
{
    internal class PlayerPrefsStorage : StorageBase
    {
        public override void Save<T>(T data, string key)
        {
            var dataJson = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(key, dataJson);
        }

        public override bool TryLoad<T>(string key, out T data)
        {
            data = default;
            var playerPrefsData = PlayerPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(playerPrefsData))
            {
                return false;
            }

            data = JsonConvert.DeserializeObject<T>(playerPrefsData);
            return true;
        }

        public override void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}