namespace TaobaoExpress.Services.BusinessRules.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules.Implementation;

    public class UpdatedBusinessRule : BusinessRuleBase<IUpdated>
    {
        public override void PreSave(IList<IUpdated> added, IList<IUpdated> updated, IList<IUpdated> removed)
        {
            foreach (var item in updated)
            {
                item.Updated = DateTime.Now;
            }

            foreach (var item in added)
            {
                item.Updated = SqlDateTime.MinValue.Value;
            }
        }
    }
}
