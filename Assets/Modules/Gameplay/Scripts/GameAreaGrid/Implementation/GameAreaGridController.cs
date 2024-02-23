using System;
using System.Collections.Generic;
using System.Threading;
using Core.DI.Attributes;
using Core.MVC.Implementation;
using Core.Swipe.Scripts.Data;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.Data;
using Modules.Gameplay.Scripts.GameAreaGrid.Declaration;
using Modules.Gameplay.Scripts.GameElement.Data.Block;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using Modules.Level.Declaration;
using Modules.SpawnFactory.Declaration;
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
        private BlockItemPoolObject[,] _blocks;
        private BlockItemPoolObject _pressedBlock;
        private List<BlockItemPoolObject> _alteredBlocks;
        private CancellationTokenSource _cancellationTokenSource;
        
        public void RestartLevel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            DestroyAllBlocks();
            ShowViewAsync().Forget();
        }

        public void ShowNextLevel()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            DestroyAllBlocks();
            _levelService.NextLevel();
            ShowViewAsync().Forget();
        }

        private void DestroyAllBlocks()
        {
            var columns = _blocks.GetLength(0);
            var rows = _blocks.GetLength(1);
            
            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (_blocks[column, row] == null)
                    {
                        continue;
                    }
                    
                    _spawnFactoryService.Destroy(_blocks[column, row]);
                    _blocks[column, row] = null;
                }
            }
        }


        protected override UniTask InitializeControllerAsync()
        {
            _blocksContainer = Resources.Load<BlocksContainer>(AssetResources.BlocksContainerPath);
            View.SwipeEnded += SwipeEnded;
            
            return UniTask.CompletedTask;
        }

        protected override UniTask ShowViewAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _currentLevel = _levelService.CurrentLevel;
            View.InitializeGrid(_currentLevel);
            var columns = _currentLevel.GetLength(0);
            var rows = _currentLevel.GetLength(1);
            _blocks = new BlockItemPoolObject[columns, rows];

            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (_currentLevel[column, row] == 0)
                    {
                        continue;
                    }

                    InitializeBlocks(column, row);
                }
            }

            return UniTask.CompletedTask;
        }

        protected override UniTask HideViewAsync()
        {
            View.SwipeEnded -= SwipeEnded;
            return UniTask.CompletedTask;
        }
        
        private void InitializeBlocks(int column, int row)
        {
            var block = GetInitializedBlock(_currentLevel[column, row]);
            if (block == null)
            {
                Debug.LogError($"Block with BlockId - {_currentLevel[column, row]} equal null.");
                return;
            }
                        
            block.Initialize(View.GetGridCellData(new Vector2Int(column, row)), _currentLevel[column, row]);
            _blocks[column, row] = block;
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

        private async UniTask ChangeBlockPositionAsync(Direction swipeDirection, CancellationTokenSource cancellationTokenSource)
        {
            if (CheckContainsAlteredBlock(_pressedBlock))
            {
                _pressedBlock = null;
                return;
            }
            
            var newGridPosition = _pressedBlock.CellPosition + GetOffsetGridPosition(swipeDirection);
            if (!IsLevelBoundsCheckPassed(newGridPosition))
            {
                _pressedBlock = null;
                return;
            }

            var changeableBlock = _blocks[newGridPosition.x, newGridPosition.y];
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
            
            var changeableBlock = _blocks[block.CellPosition.x, block.CellPosition.y];
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
                if (!IsFallenBlocks())
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
            var сhangeableBlock = _blocks[newPosition.x, newPosition.y];
            _currentLevel[block.CellPosition.x, block.CellPosition.y] = сhangeableBlock == null ? 0 : сhangeableBlock.Id;
            _currentLevel[newPosition.x, newPosition.y] = block.Id;
            _blocks[block.CellPosition.x, block.CellPosition.y] = сhangeableBlock;
            _blocks[newPosition.x, newPosition.y] = block;
        }

        private bool IsDestroyBlocks()
        {
            _alteredBlocks = new List<BlockItemPoolObject>();
            LinesDestroyChecker(Vector2Int.down, Vector2Int.up);
            LinesDestroyChecker(Vector2Int.left, Vector2Int.right);
            LinesDestroyChecker(Vector2Int.down, Vector2Int.up);

            return _alteredBlocks.Count > 0;
        }

        private void AddToDestroyBlocks(BlockItemPoolObject blockItem)
        {
            if (_alteredBlocks.Contains(blockItem))
            {
                return;
            }

            _alteredBlocks.Add(blockItem);
        }

        private void LinesDestroyChecker(Vector2Int prePositionOffset, Vector2Int nextPositionOffset)
        {
            for (var colum = 0; colum < _blocks.GetLength(0); colum++)
            {
                for (var row = 0; row < _blocks.GetLength(1); row++)
                {
                    if (_blocks[colum, row] == null)
                    {
                        continue;
                    }

                    if (_alteredBlocks.Contains(_blocks[colum, row]))
                    {
                        continue;
                    }
                    
                    FindDestructibleBlocks(prePositionOffset, nextPositionOffset, _blocks[colum, row]);
                }
            }
        }

        private void FindDestructibleBlocks(
            Vector2Int prePositionOffset,
            Vector2Int nextPositionOffset,
            BlockItemPoolObject block)
        {
            if (!FindDestructibleBlocks(block, prePositionOffset))
            {
                FindDestructibleBlocks(block, nextPositionOffset);
                return;
            }

            if (!FindDestructibleBlocks(block, nextPositionOffset))
            {
                return;
            }
            
            var preCell = block.CellPosition + prePositionOffset;
            var nextCell = block.CellPosition + nextPositionOffset;
            AddToDestroyBlocks(_blocks[preCell.x, preCell.y]);
            AddToDestroyBlocks(block);
            AddToDestroyBlocks(_blocks[nextCell.x, nextCell.y]);
        }
        
        private bool FindDestructibleBlocks(BlockItemPoolObject block, Vector2Int gridPositionOffset)
        {
            var cellChecked = block.CellPosition + gridPositionOffset;
            if (!IsLevelBoundsCheckPassed(cellChecked))
            {
                return false;
            }

            var blockChecked = _blocks[cellChecked.x, cellChecked.y];
            if (blockChecked == null)
            {
                return false;
            }
            
            if (blockChecked.Id != block.Id)
            {
                return false;
            }

            if (!_alteredBlocks.Contains(_blocks[cellChecked.x, cellChecked.y]))
            {
                return true;
            }
            
            AddToDestroyBlocks(block);
            return false;
        }

        private bool IsFallenBlocks()
        {
            _alteredBlocks = new List<BlockItemPoolObject>();
            for (var colum = 0; colum < _blocks.GetLength(0); colum++)
            {
                for (var row = 1; row < _blocks.GetLength(1); row++)
                {
                    if (_blocks[colum, row] == null)
                    {
                        continue;
                    }

                    var bottomCell = _blocks[colum, row].CellPosition + Vector2Int.down;
                    if (!IsLevelBoundsCheckPassed(bottomCell))
                    {
                        continue;
                    }

                    if (_alteredBlocks.Contains(_blocks[bottomCell.x, bottomCell.y]))
                    {
                        _alteredBlocks.Add(_blocks[colum, row]);
                        continue;
                    }

                    if (_blocks[bottomCell.x, bottomCell.y] != null )
                    {
                        continue;
                    }

                    _alteredBlocks.Add(_blocks[colum, row]);
                }
            }

            return _alteredBlocks.Count > 0;
        }

        private bool IsLevelBoundsCheckPassed(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 &&
                   gridPosition.x < _currentLevel.GetLength(0) &&
                   gridPosition.y >= 0 &&
                   gridPosition.y < _currentLevel.GetLength(1);
        }

        private static Vector2Int GetOffsetGridPosition(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Vector2Int.up;
                case Direction.Down:
                    return Vector2Int.down;
                case Direction.Left:
                    return Vector2Int.left;
                case Direction.Right:
                    return Vector2Int.right;
                default:
                    return Vector2Int.zero;
            }
        }

        private async UniTask DestroyBlocksAsync(CancellationTokenSource cancellationTokenSource)
        {
            if (!IsDestroyBlocks())
            {
                return;
            }

            var destroyTasks = new List<UniTask>(_alteredBlocks.Count);
            if (destroyTasks == null)
            {
                throw new ArgumentNullException(nameof(destroyTasks));
            }
            
            _alteredBlocks.ForEach(block =>
            {
                block.MouseDown -= OnMouseDown;
                var destroyUniTask = block.Deactivate(cancellationTokenSource);
                destroyTasks.Add(destroyUniTask);
            });

            await UniTask.WhenAll(destroyTasks);
            
            _alteredBlocks.ForEach(block =>
            {
                _blocks[block.CellPosition.x, block.CellPosition.y] = null;
                _currentLevel[block.CellPosition.x, block.CellPosition.y] = 0;
            });
            _spawnFactoryService.Destroy(_alteredBlocks);


            CheckWin();
            await CheckBlocksPositionsAsync(cancellationTokenSource);
        }
        
        private void CheckWin()
        {
            var columns = _blocks.GetLength(0);
            var rows = _blocks.GetLength(1);

            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (_blocks[column, row] != null)
                    {
                        return;
                    }
                }
            }

            ShowNextLevel();
        }
    }
}