using System;
using System.Threading;
using Core.Extensions;
using Core.PoolObject.Declaration;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.GameAreaGrid.Data;
using UnityEditor.Animations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Gameplay.Scripts.GameElement.PoolObjects
{
    public class BlockItemPoolObject : ItemPoolObject
    {
        private const int Speed = 5;
        private const float DistanceDelta = 0.01f;
        private const string StartAnimation = "Idle";
        private const int DestroyAnimationIndex = 1;

        public event Action<BlockItemPoolObject> MouseDown;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;

        public int Id { get; private set; }
        public Vector2Int CellPosition { get; private set; }

        private Vector3 _newCellPosition;
        private bool _isMove;

        public void ShowAnimation(AnimatorController animatorController)
        {
            _animator.runtimeAnimatorController = animatorController;
            _animator.Play(StartAnimation, 0, Random.Range(0, 1f));
        }

        public void Initialize(GridCellData gridCellData, int blockId)
        {
            Id = blockId;
            transform.localPosition = gridCellData.Position;
            _spriteRenderer.sortingOrder = gridCellData.Layer;
            CellPosition = gridCellData.GridPosition;
        }

        private void Update()
        {
            if (!_isMove)
            {
                return;
            }

            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                _newCellPosition,
                Speed * Time.deltaTime);
            _isMove = Vector3.Distance(transform.localPosition, _newCellPosition) > DistanceDelta;
        }

        public UniTask ArrangeAsync(GridCellData gridCellData)
        {
            _newCellPosition = gridCellData.Position;
            _spriteRenderer.sortingOrder = gridCellData.Layer;
            CellPosition = gridCellData.GridPosition;
            _isMove = true;

            return UniTask.WaitUntil(() => _isMove == false);
        }

        public async UniTask Deactivate(CancellationTokenSource cancellationTokenSource)
        {
            await _animator.PlayAsync(DestroyAnimationIndex, cancellationTokenSource.Token);

            _animator.runtimeAnimatorController = null;
            _spriteRenderer.sprite = null;
        }

        private void OnMouseDown()
        {
            MouseDown?.Invoke(this);
        }
    }
}