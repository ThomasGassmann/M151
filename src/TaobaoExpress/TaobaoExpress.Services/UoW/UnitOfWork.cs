namespace TaobaoExpress.Services.UoW
{
    using System;
    using TaobaoExpress.Services.Repositories;

    public class UnitOfWork : IUnitOfWork
    {
        public IRepository<T> CreateRepository<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int Save()
        {
            throw new NotImplementedException();
        }
    }
}
