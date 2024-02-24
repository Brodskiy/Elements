using System.Collections.Generic;
using System.Threading;
using Core.DI.Attributes;
using Core.MVC.Implementation;
using Core.Swipe.Scripts.Data;
using Core.Swipe.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.Data;
using Modules.Gameplay.Scripts.GameAreaGrid.Declaration;
using Modules.Gameplay.Scripts.GameElement.Data.Block;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using Modules.Gameplay.Scripts.SpawnFactory.Declaration;
using Modules.Level.Declaration;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Implementation
{
    internal class GameAreaGridController : Controller<GameAreaGridView>, IGameAreaGrid
    {
        [Inject]
        private readonly ILevelService _levelService;
        [Inject]
        private readonly ISpawnFactoryService _spawnFactoryService;

        public Transform ContainerForBlocks => View.BlocksContainer;

        protected override string ViewPrefabName => AssetResources.GameAreaGridPrefabPath;

        private int[,] _currentLevel;
        private BlocksContainer _blocksContainer;
        private BlockItemPoolObject _pressedBlock;
        private List<BlockItemPoolObject> _alteredBlocks;
        private CancellationTokenSource _cancellationTokenSource;
        private BlocksGrid _blocksGrid;

        public void RestartLevel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            DestroyAllBlocks();
            CreateGameArea();
        }

        public void ShowNextLevel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            DestroyAllBlocks();
            _levelService.NextLevel();
            CreateGameArea();
        }
        
        protected override void DoShow()
        {
            _blocksContainer = Resources.Load<BlocksContainer>(AssetResources.BlocksContainerPath);
            View.SwipeEnded += SwipeEnded;
            CreateGameArea();
        }

        protected override void DoHide()
        {
            View.SwipeEnded -= SwipeEnded;
        }

        private void CreateGameArea()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _currentLevel = _levelService.CurrentLevel;
            View.InitializeGrid(_currentLevel);
            var columns = _currentLevel.GetLength(0);
            var rows = _currentLevel.GetLength(1);
            _blocksGrid = new BlocksGrid(new Vector2Int(columns, rows));
            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (_currentLevel[column, row] == 0)
                    {
                        continue;
                    }

                    InitializeBlock(column, row);
                }
            }
        }

        private void InitializeBlock(int column, int row)
        {
            var block = GetInitializedBlock(_currentLevel[column, row]);
            if (block == null)
            {
                Debug.LogError($"Block with BlockId - {_currentLevel[column, row]} equal null.");
                return;
            }

            block.Initialize(View.GetGridCellData(new Vector2Int(column, row)), _currentLevel[column, row]);
            _blocksGrid.AddBlock(block);
        }

        private BlockItemPoolObject GetInitializedBlock(int blockIndex)
        {
            var block = _spawnFactoryService.Get<BlockItemPoolObject>();
            var blockAnimator = _blocksContainer.GetBlockAnimatorById(blockIndex);
            if (block == null || blockAnimator == null)
            {
                return null;
            }

            block.ShowAnimation(blockAnimator);
            block.MouseDown += OnMouseDown;
            return block;
        }

        private void OnMouseDown(BlockItemPoolObject pressedBlock)
        {
            _pressedBlock = pressedBlock;
        }

        private void SwipeEnded(Direction swipeDirection)
        {
            if (_pressedBlock == null)
            {
                return;
            }

            ChangeBlockPositionAsync(swipeDirection, _cancellationTokenSource).Forget();
        }

        private async UniTask ChangeBlockPositionAsync(Direction swipeDirection,
            CancellationTokenSource cancellationTokenSource)
        {
            if (CheckContainsAlteredBlock(_pressedBlock))
            {
                _pressedBlock = null;
                return;
            }

            var newGridPosition = _pressedBlock.CellPosition + swipeDirection.ToVector2Int();
            if (!IsLevelBoundsCheckPassed(newGridPosition))
            {
                _pressedBlock = null;
                return;
            }

            var changeableBlock = _blocksGrid.Blocks[newGridPosition.x, newGridPosition.y];
            if (swipeDirection == Direction.Up && changeableBlock == null)
            {
                _pressedBlock = null;
                return;
            }

            if (CheckContainsAlteredBlock(changeableBlock))
            {
                _pressedBlock = null;
                return;
            }

            await ChangeBlocksPositionAsync(_pressedBlock, newGridPosition);
            _pressedBlock = null;

            await CheckBlocksPositionsAsync(cancellationTokenSource);
            _levelService.UpdateCurrentLevel(_currentLevel);
        }

        private bool CheckContainsAlteredBlock(BlockItemPoolObject block)
        {
            return _alteredBlocks != null && _alteredBlocks.Contains(block);
        }

        private async UniTask ChangeBlocksPositionAsync(BlockItemPoolObject block, Vector2Int newCell)
        {
            var oldCellPosition = block.CellPosition;

            ChangeBlockPosition(block, newCell);

            var changeableBlock = _blocksGrid.Blocks[block.CellPosition.x, block.CellPosition.y];
            if (changeableBlock != null)
            {
                View.ArrangeBlockAsync(changeableBlock, oldCellPosition).Forget();
            }

            await View.ArrangeBlockAsync(block, newCell);
        }

        private async UniTask CheckBlocksPositionsAsync(CancellationTokenSource cancellationTokenSource)
        {
            await MoveBlocksDownAsync();
            await DestroyBlocksAsync(cancellationTokenSource);
        }

        private async UniTask MoveBlocksDownAsync()
        {
            while (true)
            {
                _alteredBlocks = _blocksGrid.GetFallenBlocks();
                if (_alteredBlocks.Count <= 0)
                {
                    break;
                }

                var tasks = new List<UniTask>();
                foreach (var block in _alteredBlocks)
                {
                    var newPosition = block.CellPosition + Vector2Int.down;
                    ChangeBlockPosition(block, newPosition);
                    tasks.Add(View.ArrangeBlockAsync(block, newPosition));
                }

                await UniTask.WhenAll(tasks);
            }
        }

        private void ChangeBlockPosition(BlockItemPoolObject block, Vector2Int newPosition)
        {
            var сhangeableBlock = _blocksGrid.Blocks[newPosition.x, newPosition.y];
            _currentLevel[block.CellPosition.x, block.CellPosition.y] = сhangeableBlock == null ? 0 : сhangeableBlock.Id;
            _currentLevel[newPosition.x, newPosition.y] = block.Id;
            _blocksGrid.Blocks[block.CellPosition.x, block.CellPosition.y] = сhangeableBlock;
            _blocksGrid.Blocks[newPosition.x, newPosition.y] = block;
        }

        private bool IsLevelBoundsCheckPassed(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 &&
                   gridPosition.x < _currentLevel.GetLength(0) &&
                   gridPosition.y >= 0 &&
                   gridPosition.y < _currentLevel.GetLength(1);
        }

        private async UniTask DestroyBlocksAsync(CancellationTokenSource cancellationTokenSource)
        {
            _alteredBlocks = _blocksGrid.GetDestructibleBlocks();
            if (_alteredBlocks.Count <= 0)
            {
                return;
            }

            var destroyTasks = new List<UniTask>(_alteredBlocks.Count);

            _alteredBlocks.ForEach(block =>
            {
                block.MouseDown -= OnMouseDown;
                var destroyUniTask = block.Deactivate(cancellationTokenSource);
                destroyTasks.Add(destroyUniTask);
            });

            await UniTask.WhenAll(destroyTasks);

            _alteredBlocks.ForEach(block =>
            {
                _blocksGrid.Blocks[block.CellPosition.x, block.CellPosition.y] = null;
                _currentLevel[block.CellPosition.x, block.CellPosition.y] = 0;
            });
            _spawnFactoryService.Destroy(_alteredBlocks);

            CheckWin();
            await CheckBlocksPositionsAsync(cancellationTokenSource);
        }

        private void CheckWin()
        {
            var columns = _blocksGrid.Blocks.GetLength(0);
            var rows = _blocksGrid.Blocks.GetLength(1);

            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (_blocksGrid.Blocks[column, row] != null)
                    {
                        return;
                    }
                }
            }

            ShowNextLevel();
        }

        private void DestroyAllBlocks()
        {
            var columns = _blocksGrid.Blocks.GetLength(0);
            var rows = _blocksGrid.Blocks.GetLength(1);

            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (_blocksGrid.Blocks[column, row] == null)
                    {
                        continue;
                    }

                    _spawnFactoryService.Destroy(_blocksGrid.Blocks[column, row]);
                    _blocksGrid.Blocks[column, row] = null;
                }
            }
        }
    }
}