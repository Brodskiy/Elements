using System.Collections.Generic;
using Modules.Gameplay.Scripts.GameAreaGrid.Implementation.Helpers;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Implementation
{
    internal class BlocksGrid
    {
        private readonly BlockItemPoolObject[,] _blocksGrid;
        private readonly FallenBlocksHelper _fallenBlocksHelper;
        private readonly DestroyBlocksHelper _destroyBlocksHelper;

        private List<BlockItemPoolObject> _alteredBlocks;

        public BlocksGrid(int columns, int rows)
        {
            _blocksGrid = new BlockItemPoolObject[columns, rows];
            _fallenBlocksHelper = new FallenBlocksHelper(_blocksGrid);
        }

        public void AddBlock(BlockItemPoolObject block)
        {
            _blocksGrid[block.CellPosition.x, block.CellPosition.y] = block;
        }

        public List<BlockItemPoolObject> GetDestructibleBlocks()
        {
            return _destroyBlocksHelper.GetDestructibleBlocks();
        }

        public List<BlockItemPoolObject> GetFallenBlocks()
        {
            return _fallenBlocksHelper.GetFallenBlocks();
        }

        public BlockItemPoolObject GetBlockByCellPosition(int cellX, int cellY)
        {
            return _blocksGrid[cellX, cellY];
        }

        public void SwapBlocks(BlockItemPoolObject block, Vector2Int сhangeableBlockPosition)
        {
            var сhangeableBlock = _blocksGrid[сhangeableBlockPosition.x, сhangeableBlockPosition.y];
            _blocksGrid[block.CellPosition.x, block.CellPosition.y] = сhangeableBlock;
            _blocksGrid[сhangeableBlockPosition.x, сhangeableBlockPosition.y] = block;
        }

        public bool IsBlocksGridEmpty()
        {
            return GetAllActiveBlocks().Count <= 0;
        }

        public IList<BlockItemPoolObject> GetAllActiveBlocks()
        {
            var activeBlocks = new List<BlockItemPoolObject>();
            var columns = _blocksGrid.GetLength(0);
            var rows = _blocksGrid.GetLength(1);

            for (var column = 0; column < columns; column++)
            {
                for (var row = 0; row < rows; row++)
                {
                    if (_blocksGrid[column, row] == null)
                    {
                        continue;
                    }

                    activeBlocks.Add(_blocksGrid[column, row]);
                }
            }

            return activeBlocks;
        }

        public bool IncludedInGrid(int cellX, int cellY)
        {
            return cellX >= 0 && cellX < _blocksGrid.GetLength(0) &&
                   cellY >= 0 && cellY < _blocksGrid.GetLength(1);
        }

        public void RemoveBlock(BlockItemPoolObject block)
        {
            _blocksGrid[block.CellPosition.x, block.CellPosition.y] = null;
        }

        public int[,] GetLevelState()
        {
            var columns = _blocksGrid.GetLength(0);
            var rows = _blocksGrid.GetLength(1);
            var levelData = new int[columns, rows];

            for (var colum = 0; colum < columns; colum++)
            {
                for (var row = 0; row < rows; row++)
                {
                    levelData[colum, row] = _blocksGrid[colum, row] == null ? 0 : _blocksGrid[colum, row].Id;
                }
            }

            return levelData;
        }
    }
}