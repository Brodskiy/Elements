using Core.Swipe.Scripts.Data;
using UnityEngine;

namespace Core.Swipe.Scripts.Extensions
{
    public static class DirectionExtension
    {
        public static Vector2Int ToVector2Int(this Direction direction)
        {
            return direction switch
            {
                Direction.Up => Vector2Int.up,
                Direction.Down => Vector2Int.down,
                Direction.Left => Vector2Int.left,
                Direction.Right => Vector2Int.right,
                _ => Vector2Int.zero
            };
        }
    }
}