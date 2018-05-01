namespace TaobaoExpress.Services.Repositories.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using TaobaoExpress.DataAccess;

    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context)
        {
        }

        protected TaobaoExpressEntities Context => this.context as TaobaoExpressEntities;

        public long? GetManufacturer(long productId)
        {
            var item = this.Context.RetailerProducts.SingleOrDefault(x => x.ProductId == productId && x.IsManufacturer);
            return item?.RetailerId;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return this.Context.Products.ToList();
        }

        public byte[] GetImage(long id)
        {
            return this.Context.Products.FirstOrDefault(x => x.Id == id)?.Image;
        }

        public IReadOnlyCollection<Product> GetPossibleRelatedProducts(long productId)
        {
            var baseQuery = this.QueryAsNoTracking().Where(x => x.Id != productId);
            return baseQuery.ToList();
        }

        public IEnumerable<Product> GetProductsPage(int page, int pageSize)
        {
            var products = this.Context.Products
                .OrderByDescending(x => x.ProductReviews.Average(j => j.Review))
                .ThenBy(x => x.Name)
                .Skip(pageSize * page)
                .Take(pageSize);
            return products.ToList();
        }

        public Product GetProductWithComments(long id)
        {
            var product = this.Context.Products.Include(x => x.ProductReviews).FirstOrDefault(x => x.Id == id);
            product.ProductReviews = product.ProductReviews.OrderByDescending(x => x.Review).ToList();
            return product;
        }

        public IEnumerable<Product> GetTopCommentedProducts(int top)
        {
            var productIds = this.Context.ProductReviews
                .GroupBy(x => x.ProductId)
                .OrderByDescending(x => x.Average(p => p.Review))
                .Select(x => x.Key)
                .Take(top)
                .ToList();
            return this.Context.Products.Where(x => productIds.Contains(x.Id)).ToList();
        }

        protected override Func<Product, bool> FindExpression(Product entity)
        {
            return x => x.Id == entity.Id;
        }
    }
}
