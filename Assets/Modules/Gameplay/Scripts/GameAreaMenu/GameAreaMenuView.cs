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

        protected override UniTask DoShowAsync()
        {
            _canvas.worldCamera = Camera.main;
            _buttonNext.onClick.AddListener(NextButtonClicked);
            _buttonRestart.onClick.AddListener(RestartButtonClicked);

            return UniTask.CompletedTask;
        }
        
        protected override UniTask DoHideAsync()
        {
            _buttonNext.onClick.RemoveListener(NextButtonClicked);
            _buttonRestart.onClick.RemoveListener(RestartButtonClicked);
            
            return UniTask.CompletedTask;
        }
    }
}