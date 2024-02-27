using System.Collections.Generic;
using Modules.Gameplay.Scripts.GameElement.PoolObjects;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Implementation.Helpers
{
    public class FallenBlocksHelper
    {
        private readonly BlockItemPoolObject[,] _blocksGrid;
        private List<BlockItemPoolObject> _alteredBlocks;

        public FallenBlocksHelper(BlockItemPoolObject[,] blocksGrid)
        {
            _blocksGrid = blocksGrid;
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