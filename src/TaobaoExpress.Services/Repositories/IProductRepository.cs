namespace TaobaoExpress.Services.Repositories
{
    using System.Collections.Generic;
    using TaobaoExpress.DataAccess;

    public interface IProductRepository : IRepository<Product>
    {
        long? GetManufacturer(long productId);

        IEnumerable<Product> GetAllProducts();

        byte[] GetImage(long id);

        IEnumerable<Product> GetTopCommentedProducts(int top);

        IEnumerable<Product> GetProductsPage(int page, int pageSize);

        IReadOnlyCollection<Product> GetPossibleRelatedProducts(long productId);

        Product GetProductWithComments(long id);
    }
}
