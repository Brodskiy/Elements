using System.Collections.Generic;
using UnityEngine;

namespace Modules.Gameplay.Scripts.Level.Data
{
    [CreateAssetMenu(fileName = "LevelsContainer", menuName = "ScriptableObjects/LevelsContainer")]
    internal class LevelContainer : ScriptableObject
    {
        [SerializeField]
        private List<LevelData> _levels;

        public int LevelsCount => _levels.Count;

        public LevelData GetLevelByIndex(int levelIndex)
        {
            if (_levels.Exists(level => level.LevelIndex == levelIndex))
            {
                return _levels.Find(level => level.LevelIndex == levelIndex);
            }

            Debug.LogError($"Level with index - {levelIndex} not found.");
            return null;
        }
    }
}