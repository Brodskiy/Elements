using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.MVC.Implementation
{
    public abstract class View : MonoBehaviour, IDisposable
    {
        public virtual void Dispose(){}

        public async UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            await ShowViewAsync();
        }

        public async UniTask HideAsync()
        {
            gameObject.SetActive(false);
            await HideViewAsync();
        }
        
        protected abstract UniTask ShowViewAsync();
        protected abstract UniTask HideViewAsync();
    }
}