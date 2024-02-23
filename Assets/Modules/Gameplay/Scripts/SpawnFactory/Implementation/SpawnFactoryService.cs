using System;
using System.Collections.Generic;
using Core.Foundation.Declaration;
using Core.PoolObject.Declaration;
using Core.PoolObject.Implementation;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.SpawnFactory.Declaration;
using UnityEngine;

namespace Modules.Gameplay.Scripts.SpawnFactory.Implementation
{
    internal class SpawnFactoryService : ISpawnFactoryService, IInitialization, IDisposable
    {
        private readonly Dictionary<Type, IPoolObject<ItemPoolObject>> _poolObjects = new();

        public async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _poolObjects.Clear();
        }

        public async UniTask SpawnAsync<TItemPoolObject>(TItemPoolObject prefab, int countOfInstance)
            where TItemPoolObject : ItemPoolObject
        {
            var parent = new GameObject
            {
                name = $"{prefab.name}Parent"
            };
            
            await SpawnAsync(prefab, parent.transform, countOfInstance);
        }
        
        public async UniTask SpawnAsync<TItemPoolObject>(TItemPoolObject prefab, Transform parent, int countOfInstance)
            where TItemPoolObject : ItemPoolObject
        {
            try
            {
                if (_poolObjects.ContainsKey(typeof(TItemPoolObject)))
                {
                    _poolObjects.TryGetValue(typeof(TItemPoolObject), out var poolObject);
                    poolObject?.Add(prefab);
                    return;
                }
                
                var newPoolObject = new PoolObject<TItemPoolObject>(prefab, countOfInstance, parent.transform);
                await newPoolObject.InitializeAsync();
                
                _poolObjects.Add(typeof(TItemPoolObject), newPoolObject);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public TItemPoolObject Get<TItemPoolObject>()
            where TItemPoolObject : ItemPoolObject
        {
            if (!TryGetPoolObjectByType<TItemPoolObject>(out var itemPoolObject))
            {
                return null;
            }

            return (TItemPoolObject)itemPoolObject.Get();
        }

        public void Destroy<TItemPoolObject>(TItemPoolObject itemPoolObject) where TItemPoolObject : ItemPoolObject
        {
            if (!TryGetPoolObjectByType<TItemPoolObject>(out var poolObject))
            {
                return;
            }
            
            poolObject.Destruct(itemPoolObject);
        }

        public void Destroy<TItemPoolObject>(List<TItemPoolObject> itemsPoolObject) where TItemPoolObject : ItemPoolObject
        {
            if (!TryGetPoolObjectByType<TItemPoolObject>(out var poolObject))
            {
                return;
            }
            
            poolObject.Destruct(itemsPoolObject);
        }

        private bool TryGetPoolObjectByType<TItemPoolObject>(out IPoolObject<ItemPoolObject> poolObject)
            where TItemPoolObject : ItemPoolObject
        {
            if (_poolObjects.TryGetValue(typeof(TItemPoolObject), out poolObject))
            {
                return true;
            }

            Debug.LogError($"Can not fount pool object by type {typeof(TItemPoolObject)}.");
            return false;
        }
    }
}