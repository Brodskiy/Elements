using System;
using System.Collections.Generic;
using Core.Foundation.Declaration;

namespace Core.DI.Implementation.Binder
{
    internal class BinderClass : IBinder
    {
        public Dictionary<Type, object> Bind<T>(T instance)  where T : IDisposable, IInitialization
        {
            var bindObjects = new Dictionary<Type, object>
            {
                {instance.GetType(), instance}
            };

            return bindObjects;
        }
    }
}