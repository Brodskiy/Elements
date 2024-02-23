using System.Diagnostics.CodeAnalysis;

namespace Core.MVC.Implementation.WithModel
{
    public abstract class Controller<TView, TModel> : Controller<TView>
        where TView : View<TModel>
        where TModel : IModel
    {
        protected TModel Model { get; private set; }

        public virtual void ApplyModel([NotNull] TModel model)
        {
            Model = model;
            View.ApplyModel(Model);
        }
    }
}