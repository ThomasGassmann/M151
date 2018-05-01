namespace TaobaoExpress.Services.BusinessRules.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using TaobaoExpress.Services.UoW;
    using Unity;

    public class BusinessRuleRegistry : IBusinessRuleRegistry
    {
        private readonly IUnityContainer unityContainer;

        private IDictionary<Type, IList<Type>> registeredEntries =
            new Dictionary<Type, IList<Type>>();

        public BusinessRuleRegistry(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.RegisterEntries();
        }

        public IEnumerable<Type> GetBusinessRulesFor(Type type)
        {
            var list = new List<Type>();
            foreach (var businessRuleGroup in this.registeredEntries)
            {
                if (businessRuleGroup.Key.GetTypeInfo().IsAssignableFrom(type))
                {
                    foreach (var value in businessRuleGroup.Value)
                    {
                        list.Add(value);
                    }
                }
            }

            // Execute least specific business rule first
            return list.Distinct().OrderBy(x => x.GetTypeInfo().IsInterface);
        }

        public IBusinessRuleBase InstantiateBusinessRule(Type type, IUnitOfWork unitOfWork)
        {
            this.ThrowIfInvalidBusinessRule(type);
            var instantiated = this.unityContainer.Resolve(type);
            return (IBusinessRuleBase)instantiated;
        }

        protected void RegisterEntries()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var businessRules = types.Where(x => x.GetInterfaces().Contains(typeof(IBusinessRuleBase)) && !x.IsInterface);
            foreach (var businessRule in businessRules)
            {
                if (!businessRule.IsAbstract && !businessRule.IsGenericType)
                {
                    var type = businessRule.BaseType.GetGenericArguments().SingleOrDefault();
                    if (businessRule.BaseType == typeof(object))
                    {
                        type = typeof(object);
                    }

                    this.RegisterEntry(type, businessRule);
                }
            }
        }

        private void ThrowIfInvalidBusinessRule(Type t)
        {
            if (!t.GetInterfaces().Contains(typeof(IBusinessRuleBase)))
            {
                throw new ArgumentException($"The business rule '{t.FullName}' must implement '{typeof(IBusinessRuleBase).FullName}'.");
            }
        }

        private void RegisterEntry(Type entityType, Type businessRuleType)
        {
            this.ThrowIfInvalidBusinessRule(businessRuleType);
            if (!this.registeredEntries.ContainsKey(entityType))
            {
                this.registeredEntries.Add(entityType, new List<Type>());
                this.registeredEntries[entityType].Add(businessRuleType);
                return;
            }

            this.registeredEntries[entityType].Add(businessRuleType);
        }
    }
}
