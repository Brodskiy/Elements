using System;
using Core.DI.Attributes;
using Core.Foundation.Declaration;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.Balloon.Declaration;
using Modules.Gameplay.Scripts.Data;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using Modules.Gameplay.Scripts.SpawnFactory.Declaration;

namespace Modules.Gameplay.Scripts.Balloon.Implementation
{
    internal class BalloonService : IInitialization, IDisposable, IBalloonService
    {
        private const int ShowBalloonDelay = 1000;

        [Inject]
        private readonly ISpawnFactoryService _spawnFactoryService;

        public async UniTask InitializeAsync()
        {
            for (var i = 0; i < SettingConstants.MaxBalloonsOnScreen; i++)
            {
                ShowBalloon();

                await UniTask.Delay(ShowBalloonDelay);
            }
        }

        private void OnBalloonOutForBoard(BalloonItemPoolObject balloon)
        {
            balloon.OutForBoard -= OnBalloonOutForBoard;
            _spawnFactoryService.Destroy(balloon);
            ShowBalloon();
        }
        
        private void ShowBalloon()
        {
            var balloon = _spawnFactoryService.Get<BalloonItemPoolObject>();
            balloon.OutForBoard += OnBalloonOutForBoard;
            balloon.StartFly();
        }

        public void Dispose()
        {
        }
    }
}