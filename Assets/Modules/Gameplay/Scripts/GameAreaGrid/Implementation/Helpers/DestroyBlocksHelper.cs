using System.Collections.Generic;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Implementation.Helpers
{
    public class DestroyBlocksHelper
    {
        private readonly BlockItemPoolObject[,] _blocksGrid;
        private List<BlockItemPoolObject> _alteredBlocks;

        public List<BlockItemPoolObject> GetDestructibleBlocks()
        {
            _alteredBlocks = new List<BlockItemPoolObject>();
            LinesDestroyChecker(Vector2Int.down, Vector2Int.up);
            LinesDestroyChecker(Vector2Int.left, Vector2Int.right);
            LinesDestroyChecker(Vector2Int.down, Vector2Int.up);

            return _alteredBlocks;
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

        private bool IsLevelBoundsCheckPassed(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 &&
                   gridPosition.x < _blocksGrid.GetLength(0) &&
                   gridPosition.y >= 0 &&
                   gridPosition.y < _blocksGrid.GetLength(1);
        }
    }
}