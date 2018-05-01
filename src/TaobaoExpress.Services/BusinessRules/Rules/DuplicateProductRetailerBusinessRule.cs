namespace TaobaoExpress.Services.BusinessRules.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules.Implementation;

    public class DuplicateProductRetailerBusinessRule : BusinessRuleBase<RetailerProduct>
    {
        public override void PreSave(IList<RetailerProduct> added, IList<RetailerProduct> updated, IList<RetailerProduct> removed)
        {
            var duplicates = new List<RetailerProduct>();
            var duplicatesInAdd = added.GroupBy(x => x.ProductId).Where(x => x.Count() > 1);
            if (duplicatesInAdd.Any())
            {
                var values = duplicatesInAdd.SelectMany(x => x.ToList()).GroupBy(x => new { x.ProductId, x.RetailerId }).Select(x => x.First());
                duplicates.AddRange(values);
            }

            if (duplicates.Any())
            {
                var ids = string.Join(", ", duplicates.Select(x => $"(Product: {x.ProductId}, Retailer: {x.RetailerId})"));
                var message = $"There are duplicate products: {ids}";
                throw new BusinessRuleException(message);
            }
        }
    }
}
