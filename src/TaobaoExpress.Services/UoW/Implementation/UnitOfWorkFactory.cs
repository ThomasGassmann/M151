namespace TaobaoExpress.Services.UoW.Implementation
{
    using System;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules;
    using Unity;

    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IBusinessRuleRegistry businessRuleRegistry;

        private readonly IUnityContainer unityContainer;

        public UnitOfWorkFactory(IBusinessRuleRegistry businessRuleRegistry, IUnityContainer unityContainer)
        {
            this.businessRuleRegistry = businessRuleRegistry;
            this.unityContainer = unityContainer;
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            var context = this.CreateContext();
            return new UnitOfWork(context, this.businessRuleRegistry, this.unityContainer);
        }

        private TaobaoExpressEntities CreateContext()
        {
            return new TaobaoExpressEntities();
        }
    }
}
