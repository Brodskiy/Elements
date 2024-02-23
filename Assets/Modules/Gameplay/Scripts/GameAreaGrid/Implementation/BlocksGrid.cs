using System.Collections.Generic;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Implementation
{
    internal class BlocksGrid
    {
        public BlockItemPoolObject[,] Blocks { get; }

        private List<BlockItemPoolObject> _alteredBlocks;

        public BlocksGrid(Vector2Int arraySize)
        {
            Blocks = new BlockItemPoolObject[arraySize.x, arraySize.y];
        }

        public void AddBlock(BlockItemPoolObject block)
        {
            Blocks[block.CellPosition.x, block.CellPosition.y] = block;
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
            for (var colum = 0; colum < Blocks.GetLength(0); colum++)
            {
                for (var row = 1; row < Blocks.GetLength(1); row++)
                {
                    AddToFallenBlockList(colum, row);
                }
            }

            return _alteredBlocks;
        }
        
        private void LinesDestroyChecker(Vector2Int prePositionOffset, Vector2Int nextPositionOffset)
        {
            for (var colum = 0; colum < Blocks.GetLength(0); colum++)
            {
                for (var row = 0; row < Blocks.GetLength(1); row++)
                {
                    if (Blocks[colum, row] == null)
                    {
                        continue;
                    }

                    if (_alteredBlocks.Contains(Blocks[colum, row]))
                    {
                        continue;
                    }

                    var block = Blocks[colum, row];
                    FindDestructibleBlocks(
                        block.CellPosition + prePositionOffset,
                        block.CellPosition + nextPositionOffset,
                        block);
                }
            }
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

            AddToDestroyBlocks(Blocks[preCell.x, preCell.y]);
            AddToDestroyBlocks(block);
            AddToDestroyBlocks(Blocks[nextCell.x, nextCell.y]);
        }
        
        private bool FindDestructibleBlocks(BlockItemPoolObject block, Vector2Int cellChecked)
        {
            if (!IsLevelBoundsCheckPassed(cellChecked))
            {
                return false;
            }

            var blockChecked = Blocks[cellChecked.x, cellChecked.y];
            if (blockChecked == null)
            {
                return false;
            }

            if (blockChecked.Id != block.Id)
            {
                return false;
            }

            if (_alteredBlocks.Contains(Blocks[cellChecked.x, cellChecked.y]))
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
            if (Blocks[cellX, cellY] == null)
            {
                return;
            }

            var bottomCell = Blocks[cellX, cellY].CellPosition + Vector2Int.down;

            if (!IsLevelBoundsCheckPassed(bottomCell))
            {
                return;
            }

            if (_alteredBlocks.Contains(Blocks[bottomCell.x, bottomCell.y]))
            {
                _alteredBlocks.Add(Blocks[cellX, cellY]);
                return;
            }

            if (Blocks[bottomCell.x, bottomCell.y] != null)
            {
                return;
            }

            _alteredBlocks.Add(Blocks[cellX, cellY]);
        }
        
        private bool IsLevelBoundsCheckPassed(Vector2Int gridPosition)
        {
            return gridPosition.x >= 0 &&
                   gridPosition.x < Blocks.GetLength(0) &&
                   gridPosition.y >= 0 &&
                   gridPosition.y < Blocks.GetLength(1);
        }
    }
}