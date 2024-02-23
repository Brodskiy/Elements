using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Core.DI.Log.Logger;

namespace Core.DI.Implementation
{
    internal sealed class RootInstaller : MonoBehaviour
    {
        [SerializeField]
        private ContainerBase _container;
        
        private void Start()
        {
            try
            {
                _container.InitializeAsync().Forget();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
                throw;
            }
        }

        private void OnDestroy()
        {
            _container.Dispose();
        }
    }
}