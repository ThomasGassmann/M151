namespace TaobaoExpress.Services.Repositories
{
    using System.Collections.Generic;
    using TaobaoExpress.DataAccess;

    public interface IRelatedProductRepository : IRepository<RelatedProduct>
    {
        void CreateRelatedProduct(long product, long relatedProduct, bool isSubstitute);

        bool IsDuplicateExisting(long productId, long relatedProductId);

        IEnumerable<RelatedProduct> GetRelatedProducts(long productId);
    }
}
