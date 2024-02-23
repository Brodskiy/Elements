using Core.MVC.Implementation;
using Cysharp.Threading.Tasks;
using Modules.Gameplay.Scripts.Data;
using UnityEngine.Events;

namespace Modules.Gameplay.Scripts.GameAreaMenu
{
    internal class GameAreaMenuController : Controller<GameAreaMenuView>, IGameAreaMenu
    {
        public event UnityAction RestartButtonClicked;
        public event UnityAction NextButtonClicked;

        protected override string ViewPrefabName => AssetResources.GameAreaMenuPrefabPath;

        protected override UniTask InitializeControllerAsync()
        {
            return UniTask.CompletedTask;
        }

        protected override UniTask ShowViewAsync()
        {
            View.RestartButtonClicked += RestartButtonClicked;
            View.NextButtonClicked += NextButtonClicked;
            
            return UniTask.CompletedTask;
        }

        protected override UniTask HideViewAsync()
        {
            View.RestartButtonClicked -= RestartButtonClicked;
            View.NextButtonClicked -= NextButtonClicked;
            
            return UniTask.CompletedTask;
        }
    }
}