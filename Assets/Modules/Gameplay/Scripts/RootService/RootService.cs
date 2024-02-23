using System;
using System.Collections.Generic;
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
            await ShowViewsAsync();
        }
        
        public void Dispose()
        {
            _gameAreaMenu.NextButtonClicked -= OnNextButtonClicked;
            _gameAreaMenu.RestartButtonClicked -= OnRestartButtonClicked;
            _applicationLifecycle.ApplicationQuit -= ApplicationQuit;
        }

        private UniTask SpawnPoolObjectsAsync()
        {
            var balloonBluePrefab = Resources.Load<BalloonItemPoolObject>(AssetResources.BalloonBluePrefabPath);
            var balloonOrangePrefab = Resources.Load<BalloonItemPoolObject>(AssetResources.BalloonOrangePrefabPath);
            var blockPrefab = Resources.Load<BlockItemPoolObject>(AssetResources.BlockPrefabPath);
            var tasks = new List<UniTask>
            {
                _spawnFactoryService.SpawnAsync(balloonBluePrefab, SettingConstants.BalloonDefaultPoolObjectSize),
                _spawnFactoryService.SpawnAsync(balloonOrangePrefab, SettingConstants.BalloonDefaultPoolObjectSize),
                _spawnFactoryService.SpawnAsync(
                    blockPrefab,
                    _gameAreaGrid.ContainerForBlocks,
                    SettingConstants.BlocksDefaultPoolObjectSize)
            };

            return UniTask.WhenAll(tasks);
        }

        private void SubscribeToEvents()
        {
            _gameAreaMenu.NextButtonClicked += OnNextButtonClicked;
            _gameAreaMenu.RestartButtonClicked += OnRestartButtonClicked;
            _applicationLifecycle.ApplicationQuit += ApplicationQuit;
        }

        private UniTask ShowViewsAsync()
        {
            var tasks = new List<UniTask>
            {
                _gameAreaMenu.ShowAsync(),
                _gameAreaGrid.ShowAsync()
            };
            return UniTask.WhenAll(tasks);
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