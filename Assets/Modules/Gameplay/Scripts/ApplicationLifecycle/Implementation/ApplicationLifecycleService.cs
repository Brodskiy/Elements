using System;
using Core.Foundation.Declaration;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.ApplicationLifecycle.Declaration;
using UnityEngine;

namespace Modules.Gameplay.Scripts.ApplicationLifecycle.Implementation
{
    internal class ApplicationLifecycleService : IInitialization, IDisposable, IApplicationLifecycleService
    {
        private const string ObjectName = "ApplicationLifeCycleDetector";
        public event Action ApplicationQuit;
        public event Action<bool> ApplicationPause;
        public event Action<bool> ApplicationFocus;

        private ApplicationLifeCycleDetector _applicationLifeCycleDetector;
 
        public UniTask InitializeAsync()
        {
            var lifeCycleDetector = new GameObject(ObjectName, typeof(ApplicationLifeCycleDetector));
            _applicationLifeCycleDetector = lifeCycleDetector.GetComponent<ApplicationLifeCycleDetector>();
            _applicationLifeCycleDetector.ApplicationQuit += OnApplicationQuit;
            _applicationLifeCycleDetector.ApplicationPause += OnApplicationPause;
            _applicationLifeCycleDetector.ApplicationFocus += OnApplicationFocus;

            return UniTask.CompletedTask;
        }
        
        public void Dispose()
        {
            _applicationLifeCycleDetector.ApplicationQuit -= OnApplicationQuit;
            _applicationLifeCycleDetector.ApplicationPause -= OnApplicationPause;
            _applicationLifeCycleDetector.ApplicationFocus -= OnApplicationFocus;
        }

        private void OnApplicationQuit()
        {
            ApplicationQuit?.Invoke();
        }
        
        private void OnApplicationPause(bool isPaused)
        {
            ApplicationPause?.Invoke(isPaused);
        }
        
        private void OnApplicationFocus(bool isFocus)
        {
            ApplicationFocus?.Invoke(isFocus);
        }
    }
}