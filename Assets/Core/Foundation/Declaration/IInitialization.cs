using Cysharp.Threading.Tasks;

namespace Core.Foundation.Declaration
{
    public interface IInitialization
    {
        UniTask InitializeAsync();
    }
}