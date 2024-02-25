using System;
using System.Threading;
using Core.MVC.Implementation;
using Core.Swipe.Scripts.Data;
using Core.Swipe.Scripts.Implementation;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.GameAreaGrid.Data;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Implementation
{
    internal class GameAreaGridView : View
    {
        private const float YOffset = 2.8f;
        private const float XOffset = 0.25f;
        private const float HalfElementSize = 0.7f;

        public event Action<Direction> SwipeEnded;

        [SerializeField]
        private SwipeDetector _swipeDetector;
        [SerializeField]
        private Transform _blocksContainer;

        public Transform BlocksContainer => _blocksContainer;

        private GridCellData[,] _gridElements;

        protected override UniTask DoShowAsync()
        {
            _swipeDetector.SwipeEnded += SwipeEnded;
            return UniTask.CompletedTask;
        }

        protected override UniTask DoHideAsync()
        {
            _swipeDetector.SwipeEnded -= SwipeEnded;
            return UniTask.CompletedTask;
        }

        public void InitializeGrid(int[,] levelData)
        {
            var columns = levelData.GetLength(0);
            var rows = levelData.GetLength(1);
            _blocksContainer.localPosition = new Vector3(-columns * XOffset, -YOffset);
            _gridElements = new GridCellData[columns, rows];

            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    InitializeCell(column, row);
                }
            }
        }

        private void InitializeCell(int column, int row)
        {
            var posX = HalfElementSize * column;
            var posY = HalfElementSize * row;

            _gridElements[column, row] = new GridCellData(
                new Vector2Int(column, row),
                new Vector3(posX, posY, 0), 
                column + row);
        }

        public GridCellData GetGridCellData(int cellPositionX, int cellPositionY)
        {
            return _gridElements[cellPositionX, cellPositionY];
        }

        public UniTask ArrangeBlockAsync(
            BlockItemPoolObject currentBlock,
            Vector2Int newGridPosition,
            CancellationTokenSource cancellationTokenSource)
        {
            return currentBlock.ArrangeAsync(_gridElements[newGridPosition.x, newGridPosition.y], cancellationTokenSource);
        }
    }
}