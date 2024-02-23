using System;
using UnityEngine;

namespace Modules.Gameplay.Scripts.ApplicationLifecycle.Implementation
{
    internal class ApplicationLifeCycleDetector : MonoBehaviour
    {
        public event Action ApplicationQuit;
        public event Action<bool> ApplicationPause;
        public event Action<bool> ApplicationFocus;

        private void OnApplicationQuit()
        {
            ApplicationQuit?.Invoke();
        }

        private void OnApplicationPause(bool isPaused)
        {
            ApplicationPause?.Invoke(isPaused);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            ApplicationFocus?.Invoke(hasFocus);
        }
    }
}