namespace TaobaoExpress.Controllers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Web.Mvc;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.UoW;

    public class ProductsController : TaobaoExpressBaseController
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public ProductsController(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        [HttpGet]
        public ActionResult Index(int? page = 0, int? pageSize = 10)
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var products = unitOfWork.ProductRepository.GetProductsPage(page.Value, pageSize.Value);
                this.ViewBag.Page = page;
                this.ViewBag.PageSize = pageSize;
                return this.View(products);
            }
        }

        [HttpGet]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return this.View(new Product { Id = -1 });
            }

            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var product = unitOfWork.ProductRepository.Get(id);
                return this.View(product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(long? id, Product product)
        {
            this.CopyRequestFileToProductIfAvailable(product);
            return this.Save(product, (uow, entity) => this.RedirectToAction("View", new { id = entity.Id }));
        }

        [HttpGet]
        public ActionResult View(long? id)
        {
            if (id == null)
            {
                return this.Redirect(nameof(Index));
            }

            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var product = unitOfWork.ProductRepository.GetProductWithComments(id.Value);
                this.ViewBag.Related = unitOfWork.RelatedProductRepository.GetRelatedProducts(id.Value);
                return this.View(product);
            }
        }

        [HttpGet]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return this.RedirectToAction(nameof(Index));
            }

            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var product = unitOfWork.ProductRepository.Get(id);
                return this.View(product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            return this.ExecuteInUnitOfWork(unitOfWork =>
            {
                var related = unitOfWork.RelatedProductRepository.GetRelatedProducts(id);
                foreach (var relatedProduct in related)
                {
                    unitOfWork.RelatedProductRepository.Remove(relatedProduct);
                }

                unitOfWork.Save();
                var product = unitOfWork.ProductRepository.Get(id);
                unitOfWork.ProductRepository.Remove(product);
            }, uow => this.RedirectToAction(nameof(Index)), badRequestWithUnitOfWork: unitOfWork =>
            {
                var product = unitOfWork.ProductRepository.Get(id);
                return this.View(product);
            });
        }

        [HttpGet]
        public ActionResult Image(long id)
        {
            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                var image = unitOfWork.ProductRepository.GetImage(id);
                if (image == null)
                {
                    // return fancy default pic
                    var random = new Random();
                    Color randomColor() => Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    var bitmap = new Bitmap(500, 500);
                    var graph = Graphics.FromImage(bitmap);
                    graph.Clear(randomColor());
                    graph.FillRectangle(new SolidBrush(randomColor()), new Rectangle(30, 30, 50, 50));
                    graph.FillRectangle(new SolidBrush(randomColor()), new Rectangle(420, 30, 50, 50));
                    graph.FillRectangle(new SolidBrush(randomColor()), new Rectangle(50, 300, 400, 50));
                    using (var memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, ImageFormat.Jpeg);
                        return this.File(memoryStream.ToArray(), "image/jpg");
                    }
                }

                return this.File(image, "image/jpg");
            }
        }

        private void CopyRequestFileToProductIfAvailable(Product product)
        {
            if (this.Request.Files.Count > 0)
            {
                for (var i = 0; i < this.Request.Files.Count; i++)
                {
                    var file = this.Request.Files[i];
                    if (file.ContentLength == 0)
                    {
                        if (product.Id > 0)
                        {
                            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
                            {
                                product.Image = unitOfWork.ProductRepository.GetImage(product.Id);
                            }
                        }

                        continue;
                    }

                    if (file.ContentType.StartsWith("image/jpeg") && file.ContentLength > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            file.InputStream.CopyTo(memoryStream);
                            product.Image = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        this.ModelState.AddModelError("INVALID_FILE", "Please provide a JPG file");
                    }
                }
            }
        }
    }
}