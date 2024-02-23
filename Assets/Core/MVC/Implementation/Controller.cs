using System;
using Core.Foundation.Declaration;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.MVC.Implementation
{
    public abstract class Controller<TView> : IInitialization, IDisposable
        where TView : View
    {
        protected abstract string ViewPrefabName { get; }

        protected TView View { get; private set; }

        public async UniTask InitializeAsync()
        {
            var viewPref = Resources.Load<TView>(ViewPrefabName);
            View = Object.Instantiate(viewPref);
            View.name = viewPref.name;
            
            await InitializeControllerAsync();
        }

        public async UniTask ShowAsync()
        {
            await ShowViewAsync();
            await View.ShowAsync();
        }
        
        public async UniTask HideAsync()
        {
            await HideViewAsync();
            await View.HideAsync();
        }

        public void Dispose()
        {
            View.Dispose();
        }

        protected abstract UniTask InitializeControllerAsync();
        protected abstract UniTask ShowViewAsync();
        protected abstract UniTask HideViewAsync();
    }
}