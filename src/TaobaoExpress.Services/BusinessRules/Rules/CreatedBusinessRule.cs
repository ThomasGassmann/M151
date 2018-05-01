namespace TaobaoExpress.Services.BusinessRules.Rules
{
    using System;
    using System.Collections.Generic;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules.Implementation;

    public class CreatedBusinessRule : BusinessRuleBase<ICreated>
    {
        public override void PreSave(IList<ICreated> added, IList<ICreated> updated, IList<ICreated> removed)
        {
            foreach (var item in added)
            {
                item.Created = DateTime.Now;
            }
        }
    }
}
