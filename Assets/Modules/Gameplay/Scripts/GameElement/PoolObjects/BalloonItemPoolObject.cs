using System;
using Core.PoolObject.Declaration;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Gameplay.Scripts.GameElement.PoolObjects
{
    public class BalloonItemPoolObject : ItemPoolObject
    {
        public event Action<BalloonItemPoolObject> OutForBoard;

        [SerializeField]
        private Vector2 _speedRange;
        [SerializeField]
        private Vector2 _heightOccurrenceRange;
        [SerializeField]
        private Vector2 _amplitudeRange;
        [SerializeField]
        private Vector2 _frequencyRange;
        [SerializeField]
        private bool _withRotation;
        [SerializeField]
        [HideInInspector]
        private Vector2 _rotationRange;

        private float _speed;
        private float _amplitude;
        private float _frequency;
        private float _rotation;
        private float _startingY;
        private Camera _mainCamera;
        private Vector3 _duration;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            transform.Translate(_duration * (_speed * Time.deltaTime));
            var newY = _startingY + Mathf.Sin(Time.time * _frequency) * _amplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            if (_withRotation)
            {
                var rotationAngle = Mathf.Sin(Time.time * _frequency) * _rotation;
                transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }

            if (IsVisibleFromCamera())
            {
                return;
            }

            OutForBoard?.Invoke(this);
        }

        public void StartFly()
        {
            _duration = Random.Range(0, 50f) > 25 ? Vector3.left : Vector3.right;
            _speed = Random.Range(_speedRange.x, _speedRange.y);
            _startingY = Random.Range(_heightOccurrenceRange.x, _heightOccurrenceRange.y);
            _amplitude = Random.Range(_amplitudeRange.x, _amplitudeRange.y);
            _frequency = Random.Range(_frequencyRange.x, _frequencyRange.y);
            if (_withRotation)
            {
                _rotation = Random.Range(_rotationRange.x, _rotationRange.y);
            }

            var initialPosition = transform.position;
            initialPosition.x = - (_duration.x * _mainCamera.orthographicSize * _mainCamera.aspect) - _duration.x * 0.5f;
            initialPosition.y = _startingY;
            transform.position = initialPosition;
        }

        private bool IsVisibleFromCamera()
        {
            return _duration.x > 0
                ? _mainCamera.WorldToViewportPoint(transform.position).x < 1.1f
                : _mainCamera.WorldToViewportPoint(transform.position).x > -0.1f;
        }
    }
}