namespace TaobaoExpress.Services.Repositories
{
    using System.Collections.Generic;
    using TaobaoExpress.DataAccess;

    public interface IAuditLogRepository : IRepository<AuditLog>
    {
        void CreateAuditLog(object updatedEntity, string changeType);

        IEnumerable<AuditLog> GetAuditLogPage(int page, int pageSize);
    }
}
