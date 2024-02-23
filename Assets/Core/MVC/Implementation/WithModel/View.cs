using System.Diagnostics.CodeAnalysis;

namespace Core.MVC.Implementation.WithModel
{
    public abstract class View<TModel> : View where TModel : IModel
    {
        protected TModel Model { get; private set; }

        public virtual void ApplyModel([NotNull] TModel model)
        {
            Model = model;
        }
    }
}