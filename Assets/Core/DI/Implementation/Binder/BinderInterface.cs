using System;
using System.Collections.Generic;
using Core.Foundation.Declaration;
using Debug = Core.DI.Log.Logger;

namespace Core.DI.Implementation.Binder
{
    internal class BinderInterface : IBinder
    {
        public Dictionary<Type, object> Bind<TInstance>(TInstance instance) where TInstance : IDisposable, IInitialization
        {
            var bindObjects = new Dictionary<Type, object>();
            var interfaces = instance.GetType().GetInterfaces();
            
            foreach (var currentInterface in interfaces)
            {
                var isNotBindingInterface = currentInterface == typeof(IDisposable) || currentInterface == typeof(IInitialization);
        
                if (isNotBindingInterface)
                {
                    continue;
                }
        
                bindObjects.Add(currentInterface, instance);
            }

            if (bindObjects.Count != 0)
            {
                return bindObjects;
            }
            
            Debug.LogError($"You try bind by type - {instance.GetType().Name}. But instance have not implementation interface.");
            return null;
        }
    }
}