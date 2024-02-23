using System;

namespace Modules.Gameplay.Scripts.ApplicationLifecycle.Declaration
{
    public interface IApplicationLifecycleService
    {
        event Action ApplicationQuit;
        event Action<bool> ApplicationPause;
        event Action<bool> ApplicationFocus;
    }
}