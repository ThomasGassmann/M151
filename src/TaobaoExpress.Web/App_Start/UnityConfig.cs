namespace TaobaoExpress
{
    using System;
    using System.Linq;
    using System.Reflection;
    using TaobaoExpress.Services.BusinessRules;
    using TaobaoExpress.Services.BusinessRules.Implementation;
    using TaobaoExpress.Services.UoW;
    using TaobaoExpress.Services.UoW.Implementation;
    using Unity;
    using Unity.Lifetime;

    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              UnityConfig.RegisterTypes(container);
              return container;
          });
        
        public static IUnityContainer Container => container.Value;
        
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IUnitOfWorkFactory, UnitOfWorkFactory>(new SingletonLifetimeManager());
            container.RegisterType<IBusinessRuleRegistry, BusinessRuleRegistry>(new SingletonLifetimeManager());
            UnityConfig.ConfigureBusinessRules(container);
        }

        private static void ConfigureBusinessRules(IUnityContainer container)
        {
            var assembly = Assembly.GetCallingAssembly();
            var referenced = assembly.GetReferencedAssemblies().Select(Assembly.Load).ToList();
            referenced.Add(assembly);
            foreach (var currentAssembly in referenced)
            {
                var types = currentAssembly.GetTypes();
                var businessRules = types.Where(x => x.GetInterfaces().Contains(typeof(IBusinessRuleBase)) && !x.IsInterface);
                foreach (var businessRule in businessRules)
                {
                    if (!businessRule.IsAbstract && !businessRule.IsGenericType)
                    {
                        container.RegisterType(businessRule, new TransientLifetimeManager());
                    }
                }
            }
        }
    }
}