using MVCShoppingCart.Models.Data;
using MVCShoppingCart.Models.ViewModels.Shop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MVCShoppingCart.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial(string slug)
        {
            // Declare list of CategoryViewModel
            List<CategoryViewModel> categoryViewModels;

            // Init the list
            using (Db db = new Db())
            {
                categoryViewModels = db.Categories
                        .ToList().OrderBy(c => c.Sorting).Select(c => new CategoryViewModel(c)).ToList();
            }

            ViewBag.Category = slug;

            return PartialView(categoryViewModels);
        }

        public ActionResult Category(string slug)
        {
            // Declare a list of ProductViewModel
            List<ProductViewModel> productViewModelList;

            using (Db db = new Db())
            {
                // Get category id
                CategoryDto categoryDto = db.Categories.First(c => c.Slug == slug);
                var categoryId = categoryDto.Id;

                // Init the list
                productViewModelList = db.Products
                                        .ToList()
                                        .Where(p => p.CategoryId == categoryId)
                                        .Select(p => new ProductViewModel(p))
                                        .ToList();

                // Get category name
                ViewBag.Categoryname = db.Products.First(p => p.CategoryId == categoryId).CategoryName;
            }

            TempData["category"] = slug;

            return View(productViewModelList);
        }


        [ActionName("product-details")]
        public ActionResult ProductDetails(string slug)
        {
            // Declare the ProductViewModel and ProductDto
            ProductViewModel productViewModel;
            ProductDto productDto;

            // Init product id and categorySlug
            string categorySlug = "";
            int id = 0;

            using (Db db = new Db())
            {
                // Check if product exists
                productDto = db.Products.FirstOrDefault(p => p.Slug == slug);
                if (productDto == null)
                    return RedirectToAction("Index", "shop");

                // Get id and categorySlug
                id = productDto.Id;
                categorySlug = productDto.Category.Slug;

                // Init PVM
                productViewModel = new ProductViewModel(productDto);
            }
            // Get gallery images
            productViewModel.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(Path.GetFileName);

            TempData["category"] = categorySlug;

            return View("ProductDetails", productViewModel);
        }



    }
}