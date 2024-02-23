using System;

namespace Modules.Gameplay.Scripts.ApplicationLifecycle.Declaration
{
    public interface IApplicationLifecycleService
    {
        event Action ApplicationQuit;
    }
}