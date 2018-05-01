namespace TaobaoExpress.Services.Repositories
{
    using System.Collections.Generic;
    using TaobaoExpress.DataAccess;

    public interface IRetailerProductRepository : IRepository<RetailerProduct>
    {
        void DeleteItems(long retailerId);

        RetailerProduct GetRetailerProduct(long productId, long retailerId);

        IEnumerable<RetailerProduct> GetRetailerProductsForRetailer(long retailerId);
    }
}
