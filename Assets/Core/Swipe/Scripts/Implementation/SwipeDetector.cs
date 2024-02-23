using System;
using Core.Swipe.Scripts.Data;
using Core.Swipe.Scripts.Extensions;
using UnityEngine;

namespace Core.Swipe.Scripts.Implementation
{
    public class SwipeDetector : MonoBehaviour
    {
        public event Action<Direction> SwipeEnded;
        
        [SerializeField] private float _minSwipeMagnitude = 100;

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
            if (swipeDirection.magnitude < _minSwipeMagnitude)
            {
                return;
            }
            
            SwipeEnded?.Invoke(swipeDirection.ToDirection());
        }
    }
}