using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Data
{
    public class GridCellData
    {
        public readonly Vector2Int GridPosition;
        public readonly Vector3 Position;
        public readonly int Layer;

        internal GridCellData(Vector2Int gridPosition, Vector3 position, int layer)
        {
            GridPosition = gridPosition;
            Position = position;
            Layer = layer;
        }
    }
}