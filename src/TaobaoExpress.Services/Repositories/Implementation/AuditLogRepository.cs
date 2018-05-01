namespace TaobaoExpress.Services.Repositories.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules;

    public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(DbContext context) : base(context)
        {
        }

        protected TaobaoExpressEntities Context => this.context as TaobaoExpressEntities;

        public IEnumerable<AuditLog> GetAuditLogPage(int page, int pageSize)
        {
            return this.Context.AuditLog
                .OrderByDescending(x => x.Created)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void CreateAuditLog(object updatedEntity, string changeType)
        {
            if (!new[] { "I", "U", "D" }.Contains(changeType))
            {
                throw new BusinessRuleException("Change type of audit log must be either insert, update or delete");
            }

            var result = string.Empty;
            var realObjectTypeWhichIsNotProxyType = ObjectContext.GetObjectType(updatedEntity.GetType());
            using (var stream = new MemoryStream())
            {
                var overrides = new XmlAttributeOverrides();
                var attrs = new XmlAttributes
                {
                    XmlIgnore = true
                };

                var properties = realObjectTypeWhichIsNotProxyType.GetProperties();
                var collections = properties.Where(x => (typeof(IEnumerable).IsAssignableFrom(x.PropertyType) && x.PropertyType.Name == "ICollection`1") || x.PropertyType.FullName.StartsWith("TaobaoExpress.DataAccess."));
                foreach (var collection in collections)
                {
                    overrides.Add(realObjectTypeWhichIsNotProxyType, collection.Name, attrs);
                }

                var realThing = this.CreateRealType(realObjectTypeWhichIsNotProxyType, updatedEntity);
                var xml = new XmlSerializer(realObjectTypeWhichIsNotProxyType, overrides);
                xml.Serialize(stream, realThing);
                var resultArray = stream.ToArray();
                result = Encoding.Default.GetString(resultArray);
            }

            var auditLog = new AuditLog
            {
                UpdatedEntity = realObjectTypeWhichIsNotProxyType.FullName,
                UpdatedValue = result,
                UpdateType = changeType
            };
            this.Add(auditLog);
        }

        protected override Func<AuditLog, bool> FindExpression(AuditLog entity)
        {
            return x => x.Id == entity.Id;
        }

        private object CreateRealType(Type type, object proxyType)
        {
            var instance = Activator.CreateInstance(type);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(proxyType);
                property.SetValue(instance, value);
            }

            return instance;
        }
    }
}
