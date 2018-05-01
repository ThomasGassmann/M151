namespace TaobaoExpress.Services.Repositories
{
    using TaobaoExpress.DataAccess;

    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        void AddCommentToPost(long productId, string email, string text, int review);
    }
}
