namespace TaobaoExpress.Services.Repositories.Implementation
{
    using System;
    using System.Data.Entity;
    using TaobaoExpress.DataAccess;

    public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
    {
        public ProductReviewRepository(DbContext context) : base(context)
        {
        }

        public void AddCommentToPost(long productId, string email, string text, int review)
        {
            var comment = new ProductReview
            {
                ProductId = productId,
                Review = review,
                Text = text,
                UserEmail = email
            };
            this.Add(comment);
        }

        protected override Func<ProductReview, bool> FindExpression(ProductReview entity)
        {
            return x => x.Id == entity.Id;
        }
    }
}
