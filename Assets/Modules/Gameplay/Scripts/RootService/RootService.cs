using System;
using Core.DI.Attributes;
using Core.Foundation.Declaration;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.ApplicationLifecycle.Declaration;
using Modules.Gameplay.Scripts.Data;
using Modules.Gameplay.Scripts.GameAreaGrid.Declaration;
using Modules.Gameplay.Scripts.GameAreaMenu;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using Modules.Level.Declaration;
using Modules.SpawnFactory.Declaration;
using UnityEngine;

namespace Modules.Gameplay.Scripts.RootService
{
    internal class RootService : IInitialization, IDisposable
    {
        [Inject]
        private readonly ISpawnFactoryService _spawnFactoryService;
        [Inject]
        private readonly IGameAreaMenu _gameAreaMenu;
        [Inject]
        private readonly IGameAreaGrid _gameAreaGrid;
        [Inject]
        private readonly IApplicationLifecycleService _applicationLifecycle;
        [Inject]
        private readonly ILevelService _levelService;

        public async UniTask InitializeAsync()
        {
            await SpawnPoolObjectsAsync();
            SubscribeToEvents();
            ShowViews();
        }
        
        public void Dispose()
        {
            _gameAreaMenu.NextButtonClicked -= OnNextButtonClicked;
            _gameAreaMenu.RestartButtonClicked -= OnRestartButtonClicked;
            _applicationLifecycle.ApplicationQuit -= ApplicationQuit;
        }

        private async UniTask SpawnPoolObjectsAsync()
        {
            var balloonBluePrefab = Resources.Load<BalloonItemPoolObject>(AssetResources.BalloonBluePrefabPath);
            var balloonOrangePrefab = Resources.Load<BalloonItemPoolObject>(AssetResources.BalloonOrangePrefabPath);
            var blockPrefab = Resources.Load<BlockItemPoolObject>(AssetResources.BlockPrefabPath);
            
            await _spawnFactoryService.SpawnAsync(balloonBluePrefab, SettingConstants.BalloonDefaultPoolObjectSize);
            await _spawnFactoryService.SpawnAsync(balloonOrangePrefab, SettingConstants.BalloonDefaultPoolObjectSize);
            
            await _spawnFactoryService.SpawnAsync(
                blockPrefab,
                _gameAreaGrid.ContainerForBlocks,
                SettingConstants.BlocksDefaultPoolObjectSize);
        }

        private void SubscribeToEvents()
        {
            _gameAreaMenu.NextButtonClicked += OnNextButtonClicked;
            _gameAreaMenu.RestartButtonClicked += OnRestartButtonClicked;
            _applicationLifecycle.ApplicationQuit += ApplicationQuit;
        }

        private void ShowViews()
        {
            _gameAreaMenu.ShowAsync().Forget();
            _gameAreaGrid.ShowAsync().Forget();
        }

        private void OnNextButtonClicked()
        {
            _gameAreaGrid.ShowNextLevel();
        }

        private void OnRestartButtonClicked()
        {
            _levelService.Reset();
            _gameAreaGrid.RestartLevel();
        }
        
        private void ApplicationQuit()
        {
            _levelService.Save();
        }
    }
}