using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCShoppingCart.Models.Data;
using MVCShoppingCart.Models.ViewModels.Shop;

namespace MVCShoppingCart.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        public ActionResult CategoryMenuPartial()
        {
            // Declare list of CategoryViewModel
            List<CategoryViewModel> categoryViewModels;

            // Init the list
            using (Db db = new Db())
            {
                categoryViewModels = db.Categories
                        .ToList().OrderBy(c => c.Sorting).Select(c => new CategoryViewModel(c)).ToList();
            }

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

            return View(productViewModelList);
        }


        [ActionName("product-details")]
        public ActionResult ProductDetails(string slug)
        {
            // Declare the ProductViewModel and ProductDto
            ProductViewModel productViewModel;
            ProductDto productDto;

            // Init product id
            int id = 0;

            using (Db db = new Db())
            {
                // Check if product exists
                productDto = db.Products.FirstOrDefault(p => p.Slug == slug);
                if (productDto == null)
                    return RedirectToAction("Index", "shop");

                // Get id
                id = productDto.Id;

                // Init PVM
                productViewModel = new ProductViewModel(productDto);
            }
            // Get gallery images
            productViewModel.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(Path.GetFileName);

            return View("ProductDetails", productViewModel);
        }

    }
}