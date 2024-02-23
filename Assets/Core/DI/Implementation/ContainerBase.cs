using System;
using System.Collections.Generic;
using System.Reflection;
using Core.DI.Attributes;
using Core.DI.Data;
using Core.DI.Implementation.Binder;
using Core.Foundation.Declaration;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Core.DI.Log.Logger;

namespace Core.DI.Implementation
{
    public abstract class ContainerBase : ScriptableObject
    {
        private const BindingFlags BindingAttributes = BindingFlags.NonPublic
                                                       | BindingFlags.Instance
                                                       | BindingFlags.FlattenHierarchy;

        private readonly Dictionary<BinderType, IBinder> _binders = new()
        {
            {BinderType.Class, new BinderClass()},
            {BinderType.Interface, new BinderInterface()}
        };

        private readonly DiStorage _diStorage = new();

        public async UniTask InitializeAsync()
        {
            Registration();
            ExplodeLinks();

            await _diStorage.InitializeAsync();
        }

        public void Dispose()
        {
            _diStorage.Dispose();
        }

        protected abstract void Registration();

        protected void BindAsInterfaces<T>() where T : IDisposable, IInitialization, new()
        {
            var binder = GetBinderByType(BinderType.Interface);
            Bind<T>(binder);
        }

        protected void BindAsClass<T>() where T : IDisposable, IInitialization, new()
        {
            var binder = GetBinderByType(BinderType.Class);
            Bind<T>(binder);
        }

        private void Bind<T>(IBinder binder) where T : IDisposable, IInitialization, new()
        {
            var instance = _diStorage.GetInstanceByType<T>(typeof(T));
            var bindObjects = binder.Bind(instance);

            _diStorage.AddBindObjects(bindObjects);
        }

        private IBinder GetBinderByType(BinderType binderType)
        {
            if (_binders.TryGetValue(binderType, out var binder))
            {
                return binder;
            }

            Debug.LogError($"Binder by type - {binderType} not found.");
            return null;
        }


        private void ExplodeLinks()
        {
            foreach (var typeValue in _diStorage.BindObjects)
            {
                TryExplodeLinksToObject(typeValue.Key);
            }
        }

        private void TryExplodeLinksToObject(Type typeValue)
        {
            if ( _diStorage.BindObjects.TryGetValue(typeValue, out var objectValue))
            {
                ExplodeLinksToObject(objectValue);
            }
            else
            {
                Debug.LogError($"Type - {typeValue} not binding.");
            }
        }

        private void ExplodeLinksToObject(object objectValue)
        {
            var objectType = objectValue.GetType();
            var fieldInfos = objectType.GetFields(BindingAttributes);

            for (var i = 0; i <= fieldInfos.Length - 1; i++)
            {
                DoInject(objectValue, fieldInfos, i);
            }
        }

        private void DoInject(object objectValue, IReadOnlyList<FieldInfo> fieldInfos, int positionInArray)
        {
            foreach (var propertyInfoCustomAttribute in fieldInfos[positionInArray].CustomAttributes)
            {
                if (propertyInfoCustomAttribute.AttributeType != typeof(InjectAttribute))
                {
                    continue;
                }

                if (_diStorage.BindObjects.TryGetValue(fieldInfos[positionInArray].FieldType, out var linkToObject))
                {
                    fieldInfos[positionInArray].SetValue(objectValue, linkToObject);
                }
                else
                {
                    Debug.LogError($"Object by type - {fieldInfos[positionInArray].FieldType} instance not created.");
                }
            }
        }
    }
}