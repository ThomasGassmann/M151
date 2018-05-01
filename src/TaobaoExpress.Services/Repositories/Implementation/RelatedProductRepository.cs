namespace TaobaoExpress.Services.Repositories.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using TaobaoExpress.DataAccess;

    public class RelatedProductRepository : Repository<RelatedProduct>, IRelatedProductRepository
    {
        public RelatedProductRepository(DbContext context) : base(context)
        {
        }

        protected TaobaoExpressEntities Context => this.context as TaobaoExpressEntities;

        public void CreateRelatedProduct(long product, long relatedProduct, bool isSubstitute)
        {
            var relatedProductToCreate = new RelatedProduct
            {
                IsSubstitute = isSubstitute,
                ProductId = product,
                RelatedProductId = relatedProduct
            };
            this.Context.Set<RelatedProduct>().Add(relatedProductToCreate);
        }

        public IEnumerable<RelatedProduct> GetRelatedProducts(long productId)
        {
            var related = this.Context.RelatedProducts
                .Where(x => x.ProductId == productId || x.RelatedProductId == productId)
                .GroupBy(x => new { x.ProductId, x.RelatedProductId })
                .Select(x => x.FirstOrDefault());
            return related.ToList();
        }

        public bool IsDuplicateExisting(long productId, long relatedProductId)
        {
            return this.Context.RelatedProducts.Any(x => 
                (x.ProductId == productId && x.RelatedProductId == relatedProductId) ||
                (x.RelatedProductId == productId && x.ProductId == relatedProductId));
        }

        protected override Func<RelatedProduct, bool> FindExpression(RelatedProduct entity)
        {
            return x => x.ProductId == entity.ProductId && x.RelatedProductId == entity.RelatedProductId;
        }
    }
}
