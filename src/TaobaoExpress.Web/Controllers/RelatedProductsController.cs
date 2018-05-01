namespace TaobaoExpress.Controllers
{
    using System.Web.Mvc;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Model.Aggregates;
    using TaobaoExpress.Services.UoW;

    public class RelatedProductsController : TaobaoExpressBaseController
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public RelatedProductsController(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpGet]
        public ActionResult Create(long id)
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                if (unitOfWork.CreateRepository<DataAccess.Product>().Get(id) == null)
                {
                    return this.RedirectToAction("Index", "Products");
                }

                this.SetPossibleRelatedProducts(id);
                return this.View(new RelatedProduct { ProductId = id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(long id, RelatedProduct createRelated)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            this.SetPossibleRelatedProducts(createRelated.ProductId);
            return this.ExecuteInUnitOfWork(
                unitOfWork => unitOfWork.RelatedProductRepository.CreateRelatedProduct(createRelated.ProductId, createRelated.RelatedProductId, createRelated.IsSubstitute),
                unitOfWork => this.RedirectToAction("View", "Products", new { id = id }),
                () => this.View(createRelated));
        }

        private void SetPossibleRelatedProducts(long id)
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var product = unitOfWork.CreateRepository<DataAccess.Product>().Get(id);
                var possibleRelatedProducts = unitOfWork.ProductRepository.GetPossibleRelatedProducts(id);
                var aggregate = new ProductWithPossibleRelatedProducts(product, possibleRelatedProducts);
                this.ViewBag.Aggregate = aggregate;
            } 
        }
    }
}