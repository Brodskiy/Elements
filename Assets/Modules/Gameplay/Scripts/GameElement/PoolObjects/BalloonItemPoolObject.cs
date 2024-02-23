using System;
using Core.PoolObject.Declaration;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Gameplay.Scripts.GameElement.PoolObjects
{
    public class BalloonItemPoolObject : ItemPoolObject
    {
        public event Action<BalloonItemPoolObject> OutForBoard;
        
        private float _speed;
        private float _amplitude = 2f;
        private float _frequency = 1f;
        private float _rotationAmount = 10f;
        private float _startingY;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _speed = Random.Range(0.3f, 1.5f);
        }

        public void StartFly()
        {
            _startingY = Random.Range(0, 3);
            var initialPosition = transform.position;
            initialPosition.x = -_mainCamera.orthographicSize * _mainCamera.aspect - 0.5f;
            initialPosition.y = _startingY;
            transform.position = initialPosition;
        }

        private void Update()
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
            var newY = _startingY + Mathf.Sin(Time.time * _frequency) * _amplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            var rotationAngle = Mathf.Sin(Time.time * _frequency) * _rotationAmount;
            transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

            if (IsVisibleFromCamera())
            {
                return;
            }
            
            OutForBoard?.Invoke(this);
        }
        
        private bool IsVisibleFromCamera()
        {
            var screenPoint = _mainCamera.WorldToViewportPoint(transform.position);
            return screenPoint.x < 1.1; // && screenPoint.y is > 0 and < 1;
        }
    }
}