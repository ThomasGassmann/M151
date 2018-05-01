namespace TaobaoExpress.Services.BusinessRules.Rules
{
    using System.Collections.Generic;
    using System.Data.Entity.Core.Objects;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.UoW;

    public class AuditLogBusinessRule : IBusinessRuleBase
    {
        private IList<object> added;

        private IList<object> updated;

        private IList<object> deleted;

        public IUnitOfWork UnitOfWork { get; set; }

        public void PreSave(IList<object> added, IList<object> updated, IList<object> removed)
        {
            this.added = added;
            this.updated = updated;
            this.deleted = removed;
        }

        public void PostSave(IUnitOfWork unitOfWork)
        {
            var didSomething = false;
            foreach (var item in added)
            {
                didSomething = this.SaveAudit(unitOfWork, item, "I");
            }

            foreach (var item in updated)
            {
                didSomething = this.SaveAudit(unitOfWork, item, "U");
            }

            foreach (var item in deleted)
            {
                didSomething = this.SaveAudit(unitOfWork, item, "D");
            }

            // Save changes. This will execute all business rules again.
            if (didSomething)
            {
                unitOfWork.Save();
            }
        }

        public bool SaveAudit(IUnitOfWork unitOfWork, object updatedEntity, string updateType)
        {
            var realObjectTypeWhichIsNotProxyType = ObjectContext.GetObjectType(updatedEntity.GetType());

            // Don't do audit log for AuditLog.
            if (realObjectTypeWhichIsNotProxyType == typeof(AuditLog))
            {
                return false;
            }

            unitOfWork.AuditLogRepository.CreateAuditLog(updatedEntity, updateType);
            return true;
        }
    }
}
