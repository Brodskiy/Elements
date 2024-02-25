using System.Collections.Generic;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Implementation
{
    internal class BlocksGrid
    {
        private readonly BlockItemPoolObject[,] _blocksGrid;
        private List<BlockItemPoolObject> _alteredBlocks;

        public BlocksGrid(int columns, int rows)
        {
            _blocksGrid = new BlockItemPoolObject[columns, rows];
        }

        public void AddBlock(BlockItemPoolObject block)
        {
            _blocksGrid[block.CellPosition.x, block.CellPosition.y] = block;
        }

        public List<BlockItemPoolObject> GetDestructibleBlocks()
        {
            _alteredBlocks = new List<BlockItemPoolObject>();
            LinesDestroyChecker(Vector2Int.down, Vector2Int.up);
            LinesDestroyChecker(Vector2Int.left, Vector2Int.right);
            LinesDestroyChecker(Vector2Int.down, Vector2Int.up);

            return _alteredBlocks;
        }

        public List<BlockItemPoolObject> GetFallenBlocks()
        {
            _alteredBlocks = new List<BlockItemPoolObject>();
            for (var colum = 0; colum < _blocksGrid.GetLength(0); colum++)
            {
                for (var row = 1; row < _blocksGrid.GetLength(1); row++)
                {
                    AddToFallenBlockList(colum, row);
                }
            }

            return _alteredBlocks;
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

        private void LinesDestroyChecker(Vector2Int prePositionOffset, Vector2Int nextPositionOffset)
        {
            var columns = _blocksGrid.GetLength(0);
            var rows = _blocksGrid.GetLength(1);
            for (var colum = 0; colum < columns; colum++)
            {
                for (var row = 0; row < rows; row++)
                {
                    LineDestroyCheck(prePositionOffset, nextPositionOffset, colum, row);
                }
            }
        }

        private void LineDestroyCheck(Vector2Int prePositionOffset, Vector2Int nextPositionOffset, int colum, int row)
        {
            if (_blocksGrid[colum, row] == null)
            {
                return;
            }

            if (_alteredBlocks.Contains(_blocksGrid[colum, row]))
            {
                return;
            }

            var block = _blocksGrid[colum, row];
            FindDestructibleBlocks(
                preCell: block.CellPosition + prePositionOffset,
                nextCell: block.CellPosition + nextPositionOffset,
                block);
        }

        private void FindDestructibleBlocks(Vector2Int preCell, Vector2Int nextCell, BlockItemPoolObject block)
        {
            if (!FindDestructibleBlocks(block, preCell))
            {
                FindDestructibleBlocks(block, nextCell);
                return;
            }

            if (!FindDestructibleBlocks(block, nextCell))
            {
                return;
            }

            AddToDestroyBlocks(_blocksGrid[preCell.x, preCell.y]);
            AddToDestroyBlocks(block);
            AddToDestroyBlocks(_blocksGrid[nextCell.x, nextCell.y]);
        }

        private bool FindDestructibleBlocks(BlockItemPoolObject block, Vector2Int cellChecked)
        {
            if (!IsLevelBoundsCheckPassed(cellChecked))
            {
                return false;
            }

            var blockChecked = _blocksGrid[cellChecked.x, cellChecked.y];
            if (blockChecked == null)
            {
                return false;
            }

            if (blockChecked.Id != block.Id)
            {
                return false;
            }

            if (_alteredBlocks.Contains(_blocksGrid[cellChecked.x, cellChecked.y]))
            {
                AddToDestroyBlocks(block);
            }

            return true;
        }

        private void AddToDestroyBlocks(BlockItemPoolObject blockItem)
        {
            if (_alteredBlocks.Contains(blockItem))
            {
                return;
            }

            _alteredBlocks.Add(blockItem);
        }

        private void AddToFallenBlockList(int cellX, int cellY)
        {
            if (_blocksGrid[cellX, cellY] == null)
            {
                return;
            }

            var bottomCell = _blocksGrid[cellX, cellY].CellPosition + Vector2Int.down;

            if (!IsLevelBoundsCheckPassed(bottomCell))
            {
                return;
            }

            if (_alteredBlocks.Contains(_blocksGrid[bottomCell.x, bottomCell.y]))
            {
                _alteredBlocks.Add(_blocksGrid[cellX, cellY]);
                return;
            }

            if (_blocksGrid[bottomCell.x, bottomCell.y] != null)
            {
                return;
            }

            _alteredBlocks.Add(_blocksGrid[cellX, cellY]);
        }

        private bool IsLevelBoundsCheckPassed(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 &&
                   gridPosition.x < _blocksGrid.GetLength(0) &&
                   gridPosition.y >= 0 &&
                   gridPosition.y < _blocksGrid.GetLength(1);
        }
    }
}