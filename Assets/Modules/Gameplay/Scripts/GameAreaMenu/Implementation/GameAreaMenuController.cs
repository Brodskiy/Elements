using Core.MVC.Implementation;
using Modules.Gameplay.Scripts.Data;
using Modules.Gameplay.Scripts.GameAreaMenu.Declaration;
using UnityEngine.Events;

namespace Modules.Gameplay.Scripts.GameAreaMenu.Implementation
{
    internal class GameAreaMenuController : Controller<GameAreaMenuView>, IGameAreaMenu
    {
        public event UnityAction RestartButtonClicked;
        public event UnityAction NextButtonClicked;

        protected override string ViewPrefabName => AssetResources.GameAreaMenuPrefabPath;

        protected override void DoShow()
        {
            View.RestartButtonClicked += RestartButtonClicked;
            View.NextButtonClicked += NextButtonClicked;
        }

        protected override void DoHide()
        {
            View.RestartButtonClicked -= RestartButtonClicked;
            View.NextButtonClicked -= NextButtonClicked;
        }
    }
}