using System;
using System.Collections.Generic;
using System.Linq;
using Core.Foundation.Declaration;
using Core.PoolObject.Declaration;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.PoolObject.Implementation
{
    public class PoolObject<T> : IPoolObject<T>, IDisposable, IInitialization
        where T : ItemPoolObject
    {
        private readonly List<T> _instances;
        private readonly int _defaultInstancesCount;
        private readonly ItemPoolObject _prefab;
        private readonly Transform _activeObjectsContainer;
        private readonly Transform _inactiveObjectsContainer;

        public PoolObject(ItemPoolObject prefab, int defaultInstancesCount, Transform parent)
        {
            const string activeContainerName = "ActiveObjects";
            const string inactiveContainerName = "InactiveObjects";
            
            _instances = new List<T>(defaultInstancesCount);
            _defaultInstancesCount = defaultInstancesCount;
            _prefab = prefab;

            _activeObjectsContainer = CreateContainer(activeContainerName, parent);
            _inactiveObjectsContainer = CreateContainer(inactiveContainerName, parent);
        }

        public async UniTask InitializeAsync()
        {
            for (var i = 0; i < _defaultInstancesCount; i++)
            {
                CreateInstance();
            }

            await UniTask.CompletedTask;
        }

        public void Add(ItemPoolObject prefab)
        {
            for (var i = 0; i < _defaultInstancesCount; i++)
            {
                CreateInstance(prefab as T);
            }
        }

        public T Get()
        {
            foreach (var item in _instances.Where(item => !item.gameObject.activeInHierarchy))
            {
                item.gameObject.SetActive(true);
                item.gameObject.transform.SetParent(_activeObjectsContainer);
                item.transform.localPosition = Vector3.zero;
                return item;
            }

            var newItem = CreateInstance();
            newItem.gameObject.transform.SetParent(_activeObjectsContainer);
            newItem.transform.localPosition = Vector3.zero;

            return newItem;
        }
        
        public void Destruct(IEnumerable<ItemPoolObject> destructibleObjects)
        {
            foreach (var itemPoolObject in destructibleObjects)
            {
                Destruct(itemPoolObject);
            }
        }

        public void Destruct(ItemPoolObject destructibleObject)
        {
            destructibleObject.gameObject.SetActive(false);
            destructibleObject.gameObject.transform.SetParent(_inactiveObjectsContainer);
        }
        
        public void Dispose()
        {
            
            _instances.Clear();
        }
        
        private static Transform CreateContainer(string containerName, Transform parent)
        {
            var container = new GameObject(containerName);
            container.gameObject.transform.parent = parent;
            container.transform.localPosition = Vector3.zero;
            return container.gameObject.transform;
        }

        private T CreateInstance()
        {
            var newInstance = Object.Instantiate(_prefab as T, _inactiveObjectsContainer);
            newInstance.name = _prefab.name;
            newInstance.gameObject.SetActive(false);
            _instances.Add(newInstance);
            
            return newInstance;
        }
        
        private void CreateInstance(T prefab)
        {
            var newInstance = Object.Instantiate(prefab, _inactiveObjectsContainer);
            newInstance.name = _prefab.name;
            newInstance.gameObject.SetActive(false);
            _instances.Add(newInstance);
        }
    }
}