namespace TaobaoExpress.Controllers
{
    using System.Web.Mvc;
    using TaobaoExpress.Services.UoW;

    public class AuditController : TaobaoExpressBaseController
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public AuditController(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public ActionResult Index(int? page = 0, int? pageSize = 10)
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var auditLogRepository = unitOfWork.AuditLogRepository;
                var fetched = auditLogRepository.GetAuditLogPage(page.Value, pageSize.Value);
                this.ViewBag.Page = page;
                this.ViewBag.PageSize = pageSize;
                return this.View(fetched);
            }
        }
    }
}