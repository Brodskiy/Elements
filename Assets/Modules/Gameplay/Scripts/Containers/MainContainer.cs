using Core.DataStorage.Implementation;
using Core.DI.Implementation;
using Modules.Gameplay.Scripts.ApplicationLifecycle.Implementation;
using Modules.Gameplay.Scripts.Balloon.Implementation;
using Modules.Gameplay.Scripts.GameAreaGrid.Implementation;
using Modules.Gameplay.Scripts.GameAreaMenu.Implementation;
using Modules.Gameplay.Scripts.SpawnFactory.Implementation;
using Modules.Level.Implementation;
using UnityEngine;

namespace Modules.Gameplay.Scripts.Containers
{
    [CreateAssetMenu(fileName = "MainContainer", menuName = "DIContainer/MainContainer")]
    internal class MainContainer : ContainerBase
    {
        protected override void Registration()
        {
            BindAsInterfaces<DataStorage>();
            BindAsInterfaces<SpawnFactoryService>();
            BindAsInterfaces<LevelService>();
            BindAsInterfaces<ApplicationLifecycleService>();
            BindAsInterfaces<GameAreaMenuController>();
            BindAsInterfaces<GameAreaGridController>();

            BindAsClass<RootService.RootService>();
            
            BindAsInterfaces<BalloonService>();
        }
    }
}