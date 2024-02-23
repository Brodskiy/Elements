using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.MVC.Implementation
{
    public abstract class View : MonoBehaviour, IDisposable
    {
        public virtual void Dispose(){}

        public UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            return DoShowAsync();
        }

        public UniTask HideAsync()
        {
            gameObject.SetActive(false);
            return DoHideAsync();
        }
        
        protected abstract UniTask DoShowAsync();
        protected abstract UniTask DoHideAsync();
    }
}