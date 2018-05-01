namespace TaobaoExpress.Services.Repositories.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using TaobaoExpress.DataAccess;

    public class RetailerProductRepository : Repository<RetailerProduct>, IRetailerProductRepository
    {
        public RetailerProductRepository(DbContext context) : base(context)
        {
        }

        protected TaobaoExpressEntities Context => this.context as TaobaoExpressEntities;

        public RetailerProduct GetRetailerProduct(long productId, long retailerId)
        {
            return this.Context.RetailerProducts.FirstOrDefault(x => x.ProductId == productId && x.RetailerId == retailerId);
        }

        public void DeleteItems(long retailerId)
        {
            foreach (var item in this.Context.RetailerProducts.Where(x => x.RetailerId == retailerId))
            {
                this.Context.RetailerProducts.Remove(item);
            }
        }

        public IEnumerable<RetailerProduct> GetRetailerProductsForRetailer(long retailerId)
        {
            return this.Context.RetailerProducts.Where(x => x.RetailerId == retailerId).ToList();
        }

        protected override Func<RetailerProduct, bool> FindExpression(RetailerProduct entity)
        {
            return x => x.ProductId == entity.ProductId && x.RetailerId == entity.RetailerId;
        }
    }
}
