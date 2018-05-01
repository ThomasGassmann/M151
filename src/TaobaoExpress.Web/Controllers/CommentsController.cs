namespace TaobaoExpress.Controllers
{
    using System.Web.Mvc;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.UoW;

    public class CommentsController : TaobaoExpressBaseController
    {
        public CommentsController(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
        }

        [HttpGet]
        public ActionResult Create(long? id)
        {
            if (id == null)
            {
                return this.RedirectToAction("Index", "Products");
            }

            return this.View(new ProductReview { ProductId = id.Value });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductReview viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            return ExecuteInUnitOfWork(
                unitOfWork => unitOfWork.ProductReviewRepository.AddCommentToPost(viewModel.ProductId, viewModel.UserEmail, viewModel.Text, viewModel.Review),
                unitOfWork => this.RedirectToAction("View", "Products", new { id = viewModel.ProductId }),
                () => this.View());
        }
    }
}