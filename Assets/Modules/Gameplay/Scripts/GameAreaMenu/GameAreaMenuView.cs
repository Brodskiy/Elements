using Core.MVC.Implementation;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Gameplay.Scripts.GameAreaMenu
{
    internal class GameAreaMenuView : View
    {
        public event UnityAction RestartButtonClicked;
        public event UnityAction NextButtonClicked;
        
        [SerializeField]
        private Canvas _canvas;
        [SerializeField]
        private Button _buttonNext;
        [SerializeField]
        private Button _buttonRestart;

        protected override async UniTask ShowViewAsync()
        {
            _canvas.worldCamera = Camera.main;
            _buttonNext.onClick.AddListener(NextButtonClicked);
            _buttonRestart.onClick.AddListener(RestartButtonClicked);

            await UniTask.CompletedTask;
        }
        
        protected override async UniTask HideViewAsync()
        {
            _buttonNext.onClick.RemoveListener(NextButtonClicked);
            _buttonRestart.onClick.RemoveListener(RestartButtonClicked);
            
            await UniTask.CompletedTask;
        }
    }
}