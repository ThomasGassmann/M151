namespace TaobaoExpress.Services.BusinessRules.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules.Implementation;

    public class ProductReviewBusinessRule : BusinessRuleBase<ProductReview>
    {
        public override void PreSave(IList<ProductReview> added, IList<ProductReview> updated, IList<ProductReview> removed)
        {
            foreach (var item in added.Concat(updated))
            {
                if (item.Review > 5 || item.Review < 1)
                {
                    throw new BusinessRuleException("Review must be between 1 and 5 stars");
                }
            }
        }
    }
}
