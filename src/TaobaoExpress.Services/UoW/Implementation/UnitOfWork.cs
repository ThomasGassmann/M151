namespace TaobaoExpress.Services.UoW.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Transactions;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules;
    using TaobaoExpress.Services.Repositories;
    using TaobaoExpress.Services.Repositories.Implementation;
    using Unity;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;

        private readonly IBusinessRuleRegistry businessRuleRegistry;

        private readonly IDictionary<Type, Type> repositories = new Dictionary<Type, Type>();

        private readonly IDictionary<Type, object> createdRepositories = new Dictionary<Type, object>();

        public UnitOfWork(DbContext context, IBusinessRuleRegistry businessRuleRegistry, IUnityContainer unityContainer)
        {
            this.RegisterRepositories();
            this.context = context;
            this.businessRuleRegistry = businessRuleRegistry;
        }

        public IProductRepository ProductRepository => (IProductRepository)this.CreateRepository<Product>();

        public IAuditLogRepository AuditLogRepository => (IAuditLogRepository)this.CreateRepository<AuditLog>();

        public IRelatedProductRepository RelatedProductRepository => (IRelatedProductRepository)this.CreateRepository<RelatedProduct>();

        public IProductReviewRepository ProductReviewRepository => (IProductReviewRepository)this.CreateRepository<ProductReview>();

        public IRetailerRepository RetailerRepository => (IRetailerRepository)this.CreateRepository<Retailer>();

        public IRetailerProductRepository RetailerProductRepository => (IRetailerProductRepository)this.CreateRepository<RetailerProduct>();

        public IRepository<T> CreateRepository<T>() where T : class
        {
            if (!this.repositories.ContainsKey(typeof(T)))
            {
                throw new ArgumentException($"No repository for type {typeof(T).FullName} registered");
            }

            if (this.createdRepositories.ContainsKey(typeof(T)))
            {
                return (IRepository<T>)this.createdRepositories[typeof(T)];
            }

            var type = this.repositories[typeof(T)];
            var instance = (IRepository<T>)Activator.CreateInstance(type, this.context);
            this.createdRepositories.Add(typeof(T), instance);
            return instance;
        }

        public void Dispose() =>
            this.context.Dispose();

        public int Save()
        {
            // Execute all business rules in the same transaction
            using (var transaction = new TransactionScope(TransactionScopeOption.Required))
            {
                var businessRules = this.ExecutePreSaveBusinessRules();
                var result = this.context.SaveChanges();
                this.ExecutePostSaveBusinessRules(businessRules);
                transaction.Complete();
                return result;
            }
        }

        private IReadOnlyCollection<IBusinessRuleBase> ExecutePreSaveBusinessRules()
        {
            var list = new List<IBusinessRuleBase>();
            foreach (var changedEntityGroup in context.ChangeTracker.Entries().GroupBy(x => x.Entity.GetType()))
            {
                var addedEntities = changedEntityGroup.Where(x => x.State == EntityState.Added).Select(x => x.Entity).ToList();
                var changedEntities = changedEntityGroup.Where(x => x.State == EntityState.Modified).Select(x => x.Entity).ToList();
                var removedEntities = changedEntityGroup.Where(x => x.State == EntityState.Deleted).Select(x => x.Entity).ToList();
                var businessRulesToExecute = this.businessRuleRegistry.GetBusinessRulesFor(changedEntityGroup.Key).ToList();
                foreach (var businessRule in businessRulesToExecute)
                {
                    var instantiatedBusinessRule = this.businessRuleRegistry.InstantiateBusinessRule(businessRule, this);
                    instantiatedBusinessRule.UnitOfWork = this;
                    instantiatedBusinessRule.PreSave(addedEntities, changedEntities, removedEntities);
                    list.Add(instantiatedBusinessRule);
                }
            }

            return list;
        }

        private void ExecutePostSaveBusinessRules(IEnumerable<IBusinessRuleBase> businessRulesToExecute)
        {
            foreach (var businessRule in businessRulesToExecute)
            {
                businessRule.PostSave(this);
            }
        }

        private void RegisterRepositories()
        {
            this.repositories.Add(typeof(Product), typeof(ProductRepository));
            this.repositories.Add(typeof(AuditLog), typeof(AuditLogRepository));
            this.repositories.Add(typeof(Retailer), typeof(RetailerRepository));
            this.repositories.Add(typeof(RelatedProduct), typeof(RelatedProductRepository));
            this.repositories.Add(typeof(RetailerProduct), typeof(RetailerProductRepository));
            this.repositories.Add(typeof(ProductReview), typeof(ProductReviewRepository));
        }
    }
}
