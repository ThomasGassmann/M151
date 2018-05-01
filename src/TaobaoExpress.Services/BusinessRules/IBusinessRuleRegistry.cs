namespace TaobaoExpress.Services.BusinessRules
{
    using System;
    using System.Collections.Generic;
    using TaobaoExpress.Services.UoW;

    public interface IBusinessRuleRegistry
    {
        IEnumerable<Type> GetBusinessRulesFor(Type type);

        IBusinessRuleBase InstantiateBusinessRule(Type type, IUnitOfWork unitOfWork);
    }
}
