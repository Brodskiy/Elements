using System;
using UnityEngine;

namespace Modules.Gameplay.Scripts.ApplicationLifecycle.Implementation
{
    internal class ApplicationLifeCycleDetector : MonoBehaviour
    {
        public event Action ApplicationQuit;

        private void OnApplicationQuit()
        {
            ApplicationQuit?.Invoke();
        }
    }
}