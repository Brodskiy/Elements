using System;
using Core.Swipe.Scripts.Data;
using Core.Swipe.Scripts.Extensions;
using UnityEngine;

namespace Core.Swipe.Scripts.Implementation
{
    public class SwipeDetector : MonoBehaviour
    {
        private const float MinSwipeMagnitude = 100;

        public event Action<Direction> SwipeEnded;

        private bool _isTouch;
        private Vector2 _startPosition;
        
        private void Update()
        {
            if (!_isTouch && Input.GetMouseButtonDown(0))
            {
                _isTouch = true;
                _startPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            if (!_isTouch || !Input.GetMouseButtonUp(0))
            {
                return;
            }
            
            var finishPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _isTouch = false;
            
            OnSwipeEnded(_startPosition, finishPosition);
        }
        
        private void OnSwipeEnded(Vector2 startPosition, Vector2 finishPosition)
        {
            var swipeDirection = finishPosition - startPosition;
            if (swipeDirection.magnitude < MinSwipeMagnitude)
            {
                return;
            }
            
            SwipeEnded?.Invoke(swipeDirection.ToDirection());
        }
    }
}