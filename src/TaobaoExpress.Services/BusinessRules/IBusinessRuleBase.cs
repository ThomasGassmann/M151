namespace TaobaoExpress.Services.BusinessRules
{
    using System.Collections.Generic;
    using TaobaoExpress.Services.UoW;

    public interface IBusinessRuleBase
    {
        IUnitOfWork UnitOfWork { get; set; }

        void PreSave(IList<object> added, IList<object> updated, IList<object> removed);

        void PostSave(IUnitOfWork unitOfWork);
    }
}
