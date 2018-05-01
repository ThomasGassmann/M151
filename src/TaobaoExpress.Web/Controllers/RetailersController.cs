namespace TaobaoExpress.Controllers
{
    using System.Web.Mvc;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.UoW;

    public class RetailersController : TaobaoExpressBaseController
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public RetailersController(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpPost]
        public ActionResult Delete(long id)
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var retailer = unitOfWork.RetailerRepository.Get(id);
                if (retailer == null)
                {
                    return this.RedirectToAction(nameof(Index));
                }

                unitOfWork.RetailerRepository.Remove(retailer);
                unitOfWork.Save();
                return this.RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Index(int? page = 0, int? pageSize = 10)
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var products = unitOfWork.RetailerRepository.GetRetailersPage(page.Value, pageSize.Value);
                this.ViewBag.Page = page;
                this.ViewBag.PageSize = pageSize;
                return this.View(products);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Retailer retailer)
        {
            return this.Save(retailer, (uow, entity) => this.RedirectToAction(nameof(Index)));
        }

        [HttpGet]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                var product = new Retailer { Id = -1 };
                return this.View(product);
            }

            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var product = unitOfWork.RetailerRepository.Get(id);
                return this.View(product);
            }
        }
    }
}