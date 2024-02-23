using System;
using UnityEngine;

namespace Modules.Level.Data
{
    [Serializable]
    internal class LevelData
    {
        [Header("Indicate level")]
        [SerializeField]
        private int _level;
        [Header("Indicate the size of the game area.")]
        [SerializeField]
        private int _columns;
        [SerializeField]
        private int _rows;
        [Header("Enter an array of values starting at the\n lower left corner and moving upward.")]
        [SerializeField]
        private int[] _placementData;

        public int LevelIndex => _level;
        public int Columns => _columns;
        public int Rows => _rows;
        public int[] PlacementData => _placementData;
    }
}