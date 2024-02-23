using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.Gameplay.Scripts.GameAreaGrid.Declaration
{
    public interface IGameAreaGrid
    {
        Transform ContainerForBlocks { get; }

        UniTask ShowAsync();
        void RestartLevel();
        void ShowNextLevel();
    }
}