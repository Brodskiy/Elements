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
        protected abstract string PrefabName { get; }
        protected TView View { get; private set; }

        public UniTask InitializeAsync()
        {
            var viewPref = Resources.Load<TView>(PrefabName);
            View = Object.Instantiate(viewPref);
            View.name = viewPref.name;

            return UniTask.CompletedTask;
        }

        public UniTask ShowAsync()
        {
            DoShow();
            return View.ShowAsync();
        }

        public UniTask HideAsync()
        {
            DoHide();
            return View.HideAsync();
        }

        public void Dispose()
        {
            View.Dispose();
        }

        protected abstract void DoShow();
        protected abstract void DoHide();
    }
}