namespace TaobaoExpress.Model.Aggregates
{
    using System.Collections.Generic;
    using System.Linq;
    using TaobaoExpress.DataAccess;

    public class ProductWithPossibleRelatedProducts
    {
        public ProductWithPossibleRelatedProducts(Product product, IEnumerable<Product> related)
        {
            this.Product = product;
            this.PossibleRelatedProducts = related.ToList();
        }

        public Product Product { get; set; }

        public IReadOnlyCollection<Product> PossibleRelatedProducts { get; set; }
    }
}