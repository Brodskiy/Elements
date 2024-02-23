using Cysharp.Threading.Tasks;
using UnityEngine.Events;

namespace Modules.Gameplay.Scripts.GameAreaMenu
{
    public interface IGameAreaMenu
    {
        public event UnityAction RestartButtonClicked;
        public event UnityAction NextButtonClicked;
        
        UniTask ShowAsync();
    }
}