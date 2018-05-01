namespace TaobaoExpress.Services.Repositories.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using TaobaoExpress.DataAccess;

    public class RetailerRepository : Repository<Retailer>, IRetailerRepository
    {
        public RetailerRepository(DbContext context) : base(context)
        {
        }

        protected TaobaoExpressEntities Context => this.context as TaobaoExpressEntities;

        public IEnumerable<Retailer> GetRetailersPage(int page, int pageSize)
        {
            var retailers = this.Context.Retailers
                .OrderBy(x => x.Name)
                .Skip(pageSize * page)
                .Take(pageSize);
            return retailers.ToList();
        }

        protected override Func<Retailer, bool> FindExpression(Retailer entity)
        {
            return x => x.Id == entity.Id;
        }
    }
}
