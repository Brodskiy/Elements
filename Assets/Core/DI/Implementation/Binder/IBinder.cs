using Core.Foundation.Declaration;
using System.Collections.Generic;
using System;

namespace Core.DI.Implementation.Binder
{
    internal interface IBinder
    {
        public Dictionary<Type, object> Bind<T>(T instance) where T : IDisposable, IInitialization;
    }
}