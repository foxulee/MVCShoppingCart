using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MVCShoppingCart.Models.Data;
using MVCShoppingCart.Models.ViewModels.Shop;
using PagedList;

namespace MVCShoppingCart.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //Declare a list of models
            List<CategoryViewModel> categoryVMs = new List<CategoryViewModel>();

            //Init the list
            using (var db = new Db())
            {
                foreach (var categoryDto in db.Categories.OrderBy(c => c.Sorting).ToList())
                {
                    categoryVMs.Add(new CategoryViewModel(categoryDto));
                }
            }

            //Return view with list
            return View(categoryVMs);
        }

        // POST: Admin/Shop/DeletePage/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                var categoryDto = db.Categories.SingleOrDefault(p => p.Id == id);
                if (categoryDto == null)
                    return Content("Category do not exists.");
                db.Categories.Remove(categoryDto);
                db.SaveChanges();
            }
            return RedirectToAction("Categories");
        }

        // POST: Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Declare it
            string id;

            using (Db db = new Db())
            {
                // Check whether the category name is unique
                if (db.Categories.Any(c => c.Name == catName))
                    return "titleTaken";

                // Init dto
                CategoryDto categoryDto = new CategoryDto
                {
                    //Add to dto
                    Name = catName,
                    Slug = catName.Replace(" ", "-").ToLower(),
                    Sorting = 100
                };

                // Save dto
                db.Categories.Add(categoryDto);
                db.SaveChanges();
                // Get the id
                id = categoryDto.Id.ToString();
            }

            //Return id
            return id;
        }

        // POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (var db = new Db())
            {
                // Set initial count
                int count = 1;

                // Declare PageDto
                CategoryDto categoryDto;
                // Set sorting for each page
                foreach (var categoryId in id)
                {
                    categoryDto = db.Categories.Find(categoryId);
                    categoryDto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        // POST: Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            string slug = newCatName.Replace(" ", "-").ToLower();
            string result;
            using (Db db = new Db())
            {
                if (db.Categories.Where(c => c.Id != id).Any(c => c.Slug == slug))
                {
                    result = "titleTaken";
                    return result;
                }

                var categoryInDb = db.Categories.Single(c => c.Id == id);
                categoryInDb.Name = newCatName;
                categoryInDb.Slug = slug;
                db.SaveChanges();
                result = id.ToString();
            }

            return result;
        }

        // GET: Admin/Shop/AddProduct
        public ActionResult AddProduct()
        {
            // Init model
            ProductViewModel productViewModel = new ProductViewModel();
            // Add select list of categories to model
            using (Db db = new Db())
            {
                productViewModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            // Return view with model
            return View(productViewModel);
        }

        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductViewModel productViewModel, HttpPostedFileBase file)
        {
            // Check model status
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    productViewModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(productViewModel);
                }
            }

            // Make sure the product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(p => p.Name.ToLower() == productViewModel.Name.ToLower()))
                {
                    productViewModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That product name is taken.");
                    return View(productViewModel);
                }
            }

            // Declare product id
            int id;

            // Init and save productDto
            using (Db db = new Db())
            {
                ProductDto product = new ProductDto();
                product.Name = productViewModel.Name;
                product.Slug = productViewModel.Name.Replace(" ", "-").ToLower();
                product.Description = productViewModel.Description;
                product.Price = productViewModel.Price;
                product.CategoryId = productViewModel.CategoryId;
                product.CategoryName = db.Categories.Single(c => c.Id == productViewModel.CategoryId).Name;

                db.Products.Add(product);
                db.SaveChanges();

                // Get the id
                id = product.Id;
            }

            // Set TempData msg

            TempData["SM"] = "You have added a product!";

            #region Upload Image
            // Create necessary paths
            var originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");
            var pathStr1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathStr2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathStr3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathStr4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathStr5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathStr1))
                Directory.CreateDirectory(pathStr1);

            if (!Directory.Exists(pathStr2))
                Directory.CreateDirectory(pathStr2);

            if (!Directory.Exists(pathStr3))
                Directory.CreateDirectory(pathStr3);

            if (!Directory.Exists(pathStr4))
                Directory.CreateDirectory(pathStr4);

            if (!Directory.Exists(pathStr5))
                Directory.CreateDirectory(pathStr5);

            // Check if a file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        productViewModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(productViewModel);
                    }
                }
                // Init image name
                string imageName = file.FileName;

                // Save image name to Dto
                using (Db db = new Db())
                {
                    ProductDto product = db.Products.Single(p => p.Id == id);
                    product.ImageName = imageName;
                    db.SaveChanges();
                }

                // Set original and thumb image paths
                var path1 = $"{pathStr2}\\{imageName}";
                var path2 = $"{pathStr3}\\{imageName}";

                // Save original
                file.SaveAs(path1);

                // Create and save thumb
                var image = new WebImage(file.InputStream);
                image.Resize(200, 200);
                image.Save(path2);
            }

            #endregion

            return RedirectToAction("AddProduct");

        }

        // GET: Admin/Shop/Products
        public ActionResult Products(int? page, int? categoryId)
        {
            // Declare a list of ProductViewModel
            List<ProductViewModel> productViewModels;

            using (Db db = new Db())
            {
                // Init the list
                productViewModels = db.Products.ToList()
                    .Where(p => p.CategoryId == categoryId || categoryId == null || categoryId == 0)
                    .Select(p => new ProductViewModel(p))
                    .ToList();

                // Populate the categories select list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Set selected category
                ViewBag.SelectedCategory = categoryId.ToString();
            }

            // Set page number
            var pageNumber = page ?? 1; // if no page was specified in the query string, default to the first page (1)
            // Set pagination
            var onePageOfProducts = productViewModels.ToPagedList(pageNumber, 5); // will only contain 5 products max because of the pageSize
            ViewBag.OnePageOfProducts = onePageOfProducts;

            return View();
        }


        // GET: Admin/Shop/EditProduct/id
        public ActionResult EditProduct(int id)
        {
            // Declare productViewModel
            ProductViewModel productViewModel;

            using (Db db = new Db())
            {
                //  Get the product
                var productDto = db.Products.SingleOrDefault(p => p.Id == id);

                // Make sure product exist
                if (productDto == null)
                    return Content("That product do not exist.");

                // Init the model
                productViewModel = new ProductViewModel(productDto)
                {
                    // Make a select list
                    Categories = new SelectList(db.Categories.ToList(), "Id", "Name"),
                    // Get all gallery images
                    GalleryImages = Directory
                        .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                        .Select(Path.GetFileName)
                };
            }

            return View(productViewModel);
        }


        // POST: Admin/Shop/EditProduct/id
        [HttpPost]
        public ActionResult EditProduct(ProductViewModel productViewModel, HttpPostedFileBase file)
        {
            // Get product id
            int id = productViewModel.Id;

            // Populate categories select list and gallery images
            using (Db db = new Db())
            {
                productViewModel.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                productViewModel.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(Path.GetFileName);
            }

            // Check model state
            if (!ModelState.IsValid)
                return View(productViewModel);

            // Make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(p => p.Name == productViewModel.Name && p.Id != id))
                {
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(productViewModel);
                }
            }

            // Update product
            using (Db db = new Db())
            {
                var productDtoInDb = db.Products.Single(p => p.Id == id);
                productDtoInDb.Name = productViewModel.Name;
                productDtoInDb.Slug = productViewModel.Name.Replace(" ", "-").ToLower();
                productDtoInDb.Price = productViewModel.Price;
                productDtoInDb.Description = productViewModel.Description;
                productDtoInDb.CategoryId = productViewModel.CategoryId;
                productDtoInDb.CategoryName =
                    db.Categories.FirstOrDefault(c => c.Id == productViewModel.CategoryId)?
                    .Name;

                db.SaveChanges();
            }

            // Set TempData msg
            TempData["SM"] = "The product is updated!";

            #region Image Upload
            // Check for file upload
            if (file != null && file.ContentLength > 0)
            {
                // Get extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(productViewModel);
                    }
                }

                // Set upload directory paths
                var originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");

                var pathStr1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathStr2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // Delete files from directories
                DirectoryInfo di1 = new DirectoryInfo(pathStr1);
                DirectoryInfo di2 = new DirectoryInfo(pathStr2);

                foreach (var f in di1.GetFiles())
                {
                    f.Delete();
                }
                foreach (var f in di2.GetFiles())
                {
                    f.Delete();
                }

                // Save image name
                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    var productDtoInDb = db.Products.Single(p => p.Id == id);
                    productDtoInDb.ImageName = imageName;
                    db.SaveChanges();
                }

                // Save original and thumb images
                var path1 = $"{pathStr1}\\{imageName}";
                var path2 = $"{pathStr2}\\{imageName}";

                file.SaveAs(path1);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }


            #endregion


            return RedirectToAction("EditProduct");
        }

        // POST: Admin/Shop/DeleteProduct/id
        public ActionResult DeleteProduct(int id)
        {
            // Delete product from Db
            using (Db db = new Db())
            {
                var productDto = db.Products.SingleOrDefault(p => p.Id == id);
                if (productDto == null)
                    return Content("Product do not exists.");
                db.Products.Remove(productDto);
                db.SaveChanges();
            }

            // Delete product image folder
            var originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");
            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            return RedirectToAction("Products");
        }

        // POST: Admin/Shop/SaveGalleryImages
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Loop through files
            foreach (string fileName in Request.Files)
            {
                // Init the file
                var file = Request.Files[fileName];

                // Check it's not null
                if (file != null && file.ContentLength > 0)
                {
                    // Set directory paths
                    var originalDirectory = new DirectoryInfo($"{Server.MapPath(@"\")}Images\\Uploads");

                    string pathStr1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathStr2  = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    //Set image paths
                    var path1 = $"{pathStr1}\\{file.FileName}";
                    var path2 = $"{pathStr2}\\{file.FileName}";

                    // Save original and thumb
                    file.SaveAs(path1);
                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200);
                    img.Save(path2);
                }
            }
        }

        // POST: Admin/Shop/DeleteImage
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs/" + imageName);

            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }

       
    }
}


