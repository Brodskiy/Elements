using System.Collections.Generic;

namespace Core.PoolObject.Declaration
{
    public interface IPoolObject<out T>
        where T : ItemPoolObject
    {
        void Add(ItemPoolObject prefab);
        T Get();
        void Destruct(ItemPoolObject destructibleObject);
        void Destruct(IEnumerable<ItemPoolObject> destructibleObjects);
    }
}