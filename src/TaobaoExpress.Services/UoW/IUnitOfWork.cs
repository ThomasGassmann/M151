namespace TaobaoExpress.Services.UoW
{
    using System;
    using TaobaoExpress.Services.Repositories;

    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> CreateRepository<T>() where T : class;

        IProductRepository ProductRepository { get; }

        IAuditLogRepository AuditLogRepository { get; }

        IRelatedProductRepository RelatedProductRepository { get; }

        IProductReviewRepository ProductReviewRepository { get; }

        IRetailerRepository RetailerRepository { get; }

        IRetailerProductRepository RetailerProductRepository { get; }

        int Save();
    }
}
