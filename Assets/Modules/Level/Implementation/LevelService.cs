using System;
using Core.DataStorage.Declaration;
using Core.DI.Attributes;
using Core.Foundation.Declaration;
using Cysharp.Threading.Tasks;
using Modules.Level.Data;
using Modules.Level.Declaration;
using Modules.Level.Extensions;
using UnityEngine;

namespace Modules.Level.Implementation
{
    internal class LevelService : ILevelService, IInitialization, IDisposable
    {
        private const string LevelsContainerPath = "LevelsContainer";
        private const string LevelIndexKey = "LevelIndexData";
        private const string LevelStateDataKey = "LevelStateData";

        [Inject]
        private readonly IDataStorage _dataStorage;
        
        public int[,] CurrentLevel { get; private set; }

        private LevelContainer _levelContainer;
        private int _currentLevelIndex;

        public async UniTask InitializeAsync()
        {
            _levelContainer = Resources.Load<LevelContainer>(LevelsContainerPath);
            _currentLevelIndex = _dataStorage.TryLoad<int>(LevelIndexKey, out var level) ? level : 1;
            var levelData = _levelContainer.GetLevelByIndex(_currentLevelIndex);
            CurrentLevel = _dataStorage.TryLoad<int[]>(LevelStateDataKey, out var levelState)
                ? levelState.ToTwoDimensionalArray(levelData.Columns, levelData.Rows)
                : levelData.PlacementData.ToTwoDimensionalArray(levelData.Columns, levelData.Rows);

            await UniTask.CompletedTask;
        }
        
        public void Dispose()
        {
            CurrentLevel = null;
            _currentLevelIndex = default;
        }

        public void UpdateCurrentLevel(int[,] currentLevel)
        {
            CurrentLevel = currentLevel;
        }
        
        public void NextLevel()
        {
            if (_currentLevelIndex >= _levelContainer.LevelsCount)
            {
                _currentLevelIndex = 1;
            }
            else
            {
                _currentLevelIndex++;
            }
            
            Reset();
        }

        public void Save()
        {
            _dataStorage.Save(_currentLevelIndex, LevelIndexKey);
            _dataStorage.Save(CurrentLevel.ToArray(), LevelStateDataKey);
        }

        public void Reset()
        {
            
            var levelData = _levelContainer.GetLevelByIndex(_currentLevelIndex);
            CurrentLevel = levelData.PlacementData.ToTwoDimensionalArray(levelData.Columns, levelData.Rows);
            _dataStorage.Delete(LevelStateDataKey);
        }
    }
}