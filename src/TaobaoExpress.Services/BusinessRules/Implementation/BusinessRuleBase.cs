namespace TaobaoExpress.Services.BusinessRules.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using TaobaoExpress.Services.UoW;

    public class BusinessRuleBase<TEntity> : IBusinessRuleBase where TEntity : class
    {
        public IUnitOfWork UnitOfWork { get; set; }

        public virtual void PostSave(IUnitOfWork unitOfWork)
        {
        }

        public virtual void PreSave(IList<TEntity> added, IList<TEntity> updated, IList<TEntity> removed)
        {
        }

        public void PreSave(IList<object> added, IList<object> updated, IList<object> removed) =>
            this.PreSave(added.Cast<TEntity>().ToList(), updated.Cast<TEntity>().ToList(), removed.Cast<TEntity>().ToList());
    }
}
