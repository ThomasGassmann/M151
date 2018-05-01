namespace TaobaoExpress.Controllers
{
    using System.Web.Mvc;
    using TaobaoExpress.Services.UoW;

    public class HomeController : Controller
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public HomeController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        public ActionResult Index()
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var products = unitOfWork.ProductRepository.GetTopCommentedProducts(5);
                return this.View(products);
            }
        }
    }
}