namespace TaobaoExpress.Services.Repositories
{
    using System.Collections.Generic;
    using TaobaoExpress.DataAccess;

    public interface IRetailerRepository : IRepository<Retailer>
    {
        IEnumerable<Retailer> GetRetailersPage(int page, int pageSize);
    }
}
