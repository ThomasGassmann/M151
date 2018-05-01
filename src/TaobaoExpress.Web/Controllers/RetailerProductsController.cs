namespace TaobaoExpress.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.UoW;

    public class RetailerProductsController : TaobaoExpressBaseController
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public RetailerProductsController(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpGet]
        public ActionResult New(long? id, long amount)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                ViewBag.Amount = amount;
                ViewBag.RetailerId = id.Value;
                ViewBag.Products = unitOfWork.ProductRepository.GetAllProducts();
                return this.PartialView("~/Views/RetailerProducts/EditorTemplates/RetailerProduct.cshtml", new RetailerProduct());
            }
        }

        [HttpGet]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return this.RedirectToAction("Index", "Retailers");
            }

            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                ViewBag.RetailerId = id.Value;
                ViewBag.Products = unitOfWork.ProductRepository.GetAllProducts();
                var retailerProducts = unitOfWork.RetailerProductRepository.GetRetailerProductsForRetailer(id.Value);
                return this.View(retailerProducts);
            }
        }

        [HttpPost]
        public ActionResult Edit(long id, IEnumerable<RetailerProduct> retailerProducts)
        {
            retailerProducts = retailerProducts ?? new List<RetailerProduct>();

            if (!this.ModelState.IsValid)
            {
                return this.View(retailerProducts);
            }

            return this.ExecuteInUnitOfWork(unitOfWork =>
            {
                var repository = unitOfWork.RetailerProductRepository;

                repository.DeleteItems(id);

                foreach (var retailerProduct in retailerProducts)
                {
                    repository.Add(retailerProduct);
                }
            }, uow => this.RedirectToAction("Index", "Retailers"), badRequestWithUnitOfWork: unitOfWork =>
            {
                ViewBag.RetailerId = id;
                ViewBag.Products = unitOfWork.ProductRepository.GetAllProducts();
                return this.View(retailerProducts);
            });
        }
    }
}