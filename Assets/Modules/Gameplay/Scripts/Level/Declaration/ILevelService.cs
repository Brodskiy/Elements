namespace Modules.Level.Declaration
{
    public interface ILevelService
    {
        int[,] CurrentLevel { get; }

        void UpdateCurrentLevel(int[,] currentLevel);
        void NextLevel();
        void Save();
        void Reset();
    }
}