namespace TaobaoExpress.DataAccess
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public partial class AuditLog : ICreated 
    {
        public IDictionary<string, string> Properties
        {
            get
            {
                var xml = XElement.Parse(this.UpdatedValue);
                var ignore = new[] { "Image", "ConcurrencyCheck" };
                var descendants = xml.Descendants().Where(x => !ignore.Contains(x.Name.LocalName));
                return descendants.ToDictionary(x => x.Name.LocalName, x => x.Value);
            }
        }
    }
}
