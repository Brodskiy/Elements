using System.Collections.Generic;
using Core.PoolObject.Declaration;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.Gameplay.Scripts.SpawnFactory.Declaration
{
    public interface ISpawnFactoryService
    {
        UniTask SpawnAsync<TItemPoolObject>(TItemPoolObject prefab, int countOfInstance) where TItemPoolObject : ItemPoolObject;
        UniTask SpawnAsync<TItemPoolObject>(TItemPoolObject prefab, Transform parent, int countOfInstance) where TItemPoolObject : ItemPoolObject;
        TItemPoolObject Get<TItemPoolObject>() where TItemPoolObject : ItemPoolObject;
        void Destroy<TItemPoolObject>(TItemPoolObject itemPoolObject) where TItemPoolObject : ItemPoolObject;
        void Destroy<TItemPoolObject>(List<TItemPoolObject> itemPoolObject) where TItemPoolObject : ItemPoolObject;
    }
}