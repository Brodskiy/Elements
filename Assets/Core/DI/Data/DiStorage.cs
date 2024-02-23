using System.Collections.Generic;
using Core.Foundation.Declaration;
using Cysharp.Threading.Tasks;
using System;

namespace Core.DI.Data
{
    internal class DiStorage : IDisposable, IInitialization
    {
        public IDictionary<Type, object> BindObjects { get; } = new Dictionary<Type, object>();

        private readonly Dictionary<Type, object> _instances = new();
        private readonly List<IInitialization> _initializations = new();
        private readonly List<IDisposable> _disposables = new();

        public async UniTask InitializeAsync()
        {
            foreach (var item in _initializations)
            {
                await item.InitializeAsync();
            }
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        public T GetInstanceByType<T>(Type instanceType) where T : IInitialization, IDisposable, new()
        {
            if (_instances.TryGetValue(instanceType, out var instance))
            {
                return (T)instance;
            }

            var newInstance = new T();
            _instances.Add(newInstance.GetType(), newInstance);
            _initializations.Add(newInstance);
            _disposables.Add(newInstance);
            return newInstance;
        }

        public void AddBindObjects(Dictionary<Type, object> bindObjects)
        {
            foreach (var bindObject in bindObjects)
            {
                BindObjects.Add(bindObject.Key, bindObject.Value);
            }
        }
    }
}