namespace TaobaoExpress.Services.BusinessRules.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules.Implementation;
    using TaobaoExpress.Services.UoW;

    public class UniqueManufacturerBusinessRule : BusinessRuleBase<RetailerProduct>
    {
        public override void PreSave(IList<RetailerProduct> added, IList<RetailerProduct> updated, IList<RetailerProduct> removed)
        {
            var queryUnitOfWork = this.UnitOfWork;
            foreach (var item in added.Concat(updated).GroupBy(x => x.ProductId))
            {
                var currentManufacturer = queryUnitOfWork.ProductRepository.GetManufacturer(item.Key);
                if (currentManufacturer == null)
                {
                    continue;
                }

                var manufacturerExists = item.Any(x => x.IsManufacturer);
                if (manufacturerExists)
                {
                    var retailerId = currentManufacturer.Value;
                    var manufacturers = item.Where(x => x.IsManufacturer).GroupBy(x => x.RetailerId).Where(x => x.Key != retailerId);
                    if (manufacturers.Any())
                    {
                        throw new BusinessRuleException("One product cannot have multiple manufacturers");
                    }
                }
            }
        }
    }
}
