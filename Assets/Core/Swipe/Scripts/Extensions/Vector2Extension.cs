using Core.Swipe.Scripts.Data;
using UnityEngine;

namespace Core.Swipe.Scripts.Extensions
{
    internal static class Vector2Extension
    {
        public static Direction ToDirection(this ref Vector2 vector)
        {
            if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
            {
                return vector.x > 0 ? Direction.Right : Direction.Left;
            }

            return vector.y > 0 ? Direction.Up : Direction.Down;
        }
    }
}