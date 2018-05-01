namespace TaobaoExpress.Services.BusinessRules.Rules
{
    using System.Collections.Generic;
    using System.Linq;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules.Implementation;

    public class RelatedProductBusinessRule : BusinessRuleBase<RelatedProduct>
    {
        public override void PreSave(IList<RelatedProduct> added, IList<RelatedProduct> updated, IList<RelatedProduct> removed)
        {
            var relatedProductRepository = this.UnitOfWork.RelatedProductRepository;
            foreach (var item in added.Concat(updated))
            {
                if (item.ProductId == item.RelatedProductId)
                {
                    throw new BusinessRuleException("A product can't relate to itself");
                }

                if (relatedProductRepository.IsDuplicateExisting(item.ProductId, item.RelatedProductId))
                {
                    throw new BusinessRuleException("A product relation of this type already exists");
                }
            }
        }
    }
}
