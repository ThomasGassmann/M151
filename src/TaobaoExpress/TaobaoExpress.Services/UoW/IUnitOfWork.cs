namespace TaobaoExpress.Services.UoW
{
    using System;
    using TaobaoExpress.Services.Repositories;

    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> CreateRepository<T>() where T : class;

        int Save();
    }
}
