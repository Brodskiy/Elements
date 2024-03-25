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

        protected override string PrefabName => AssetResources.GameAreaGridPrefabPath;

        private BlocksGrid _blocksGrid;
        private BlocksContainer _blocksContainer;
        private BlockItemPoolObject _pressedBlock;
        private List<BlockItemPoolObject> _alteredBlocks;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isDestroyBlocksStarted;

        public void RestartLevel()
        {
            DestroyAllBlocks();
            CreateGameArea();
        }

        public void ShowNextLevel()
        {
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
            var currentLevel = _levelService.CurrentLevel;
            View.InitializeGrid(currentLevel);
            var columns = currentLevel.GetLength(0);
            var rows = currentLevel.GetLength(1);
            _blocksGrid = new BlocksGrid(columns, rows);
            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (currentLevel[column, row] == 0)
                    {
                        continue;
                    }

                    InitializeBlock(column, row, currentLevel[column, row]);
                }
            }
        }

        private void InitializeBlock(int column, int row, int blockId)
        {
            var block = GetInitializedBlock(blockId);
            if (block == null)
            {
                Debug.LogError($"Block with BlockId - {blockId} equal null.");
                return;
            }

            block.Initialize(View.GetGridCellData(column, row), blockId);
            _blocksGrid.AddBlock(block);
        }

        private BlockItemPoolObject GetInitializedBlock(int blockId)
        {
            var block = _spawnFactoryService.Get<BlockItemPoolObject>();
            var blockAnimator = _blocksContainer.GetBlockAnimatorById(blockId);
            if (block == null || blockAnimator == null)
            {
                return null;
            }

            block.PlayAnimation(blockAnimator);
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

            var newGridPosition = _pressedBlock.CellPosition + swipeDirection.ToVector2Int();
            if (!_blocksGrid.IncludedInGrid(newGridPosition.x, newGridPosition.y))
            {
                _pressedBlock = null;
                return;
            }

            var changeableBlock = _blocksGrid.GetBlockByCellPosition(newGridPosition.x, newGridPosition.y);
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

            await ChangeBlocksPositionAsync(_pressedBlock, newGridPosition, cancellationTokenSource);

            _pressedBlock = null;
            
            if (_isDestroyBlocksStarted)
            {
                return;
            }

            await CheckBlocksPositionsAsync(cancellationTokenSource);

            _levelService.UpdateCurrentLevel(_blocksGrid.GetLevelState());
        }

        private bool CheckContainsAlteredBlock(BlockItemPoolObject block)
        {
            return _alteredBlocks != null && _alteredBlocks.Contains(block);
        }

        private UniTask ChangeBlocksPositionAsync(BlockItemPoolObject block, Vector2Int newCell, CancellationTokenSource cancellationTokenSource)
        {
            var oldCellPosition = block.CellPosition;
            var сhangeableBlock = _blocksGrid.GetBlockByCellPosition(newCell.x, newCell.y);
            _blocksGrid.SwapBlocks(block, newCell);
            var tasks = new List<UniTask>();

            if (сhangeableBlock != null)
            {
                tasks.Add(View.ArrangeBlockAsync(сhangeableBlock, oldCellPosition, cancellationTokenSource));
            }

            tasks.Add(View.ArrangeBlockAsync(block, newCell, cancellationTokenSource));
            return UniTask.WhenAll(tasks);
        }

        private async UniTask CheckBlocksPositionsAsync(CancellationTokenSource cancellationTokenSource)
        {
            _isDestroyBlocksStarted = true;
            await MoveBlocksDownAsync(cancellationTokenSource); 
            await DestroyBlocksAsync(cancellationTokenSource);
            _isDestroyBlocksStarted = false;
        }

        private async UniTask MoveBlocksDownAsync(CancellationTokenSource cancellationTokenSource)
        {
            while (true)
            {
                _alteredBlocks = _blocksGrid.GetFallenBlocks();
                if (_alteredBlocks.Count <= 0)
                {
                    break;
                }

                var tasks = new List<UniTask>(_alteredBlocks.Count);
                foreach (var block in _alteredBlocks)
                {
                    var newPosition = block.CellPosition + Vector2Int.down;
                    _blocksGrid.SwapBlocks(block, newPosition);
                    tasks.Add(View.ArrangeBlockAsync(block, newPosition, cancellationTokenSource));
                }

                await UniTask.WhenAll(tasks);
            }
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
                destroyTasks.Add(block.Deactivate(cancellationTokenSource));
            });

            await UniTask.WhenAll(destroyTasks);

            _alteredBlocks.ForEach(block => _blocksGrid.RemoveBlock(block));
            _spawnFactoryService.Destroy(_alteredBlocks);
            CheckWin();
            await CheckBlocksPositionsAsync(cancellationTokenSource);
        }

        private void CheckWin()
        {
            if (!_blocksGrid.IsBlocksGridEmpty())
            {
                return;
            }

            ShowNextLevel();
        }

        private void DestroyAllBlocks()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _alteredBlocks?.ForEach(block => block.StopAnimation());
            _alteredBlocks?.Clear();
            _isDestroyBlocksStarted = false;

            var activeBlocks = _blocksGrid.GetAllActiveBlocks();
            foreach (var block in activeBlocks)
            {
                _spawnFactoryService.Destroy(block);
                _blocksGrid.RemoveBlock(block);
            }
        }
    }
}