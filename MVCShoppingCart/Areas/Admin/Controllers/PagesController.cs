using MVCShoppingCart.Models.Data;
using MVCShoppingCart.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVCShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declare list of PageVM
            List<PageViewModel> pageList;

            using (Db db = new Db())
            {
                //Init the list
                pageList = db.Pages.OrderBy(p => p.Sorting).ToList().Select(p => new PageViewModel(p)).ToList();
            }
            //Return view with list
            return View(pageList);
        }

        public ActionResult AddPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPage(PageViewModel pageViewModel)
        {
            if (!ModelState.IsValid)
                return View("AddPage", pageViewModel);

            using (Db db = new Db())
            {
                string slug;
                slug = String.IsNullOrWhiteSpace(pageViewModel.Slug)
                    ? pageViewModel.Title.Replace(" ", "-").ToLower()
                    : pageViewModel.Slug.ToLower();

                //make sure title and slug are unique
                if (db.Pages.Any(p => p.Title == pageViewModel.Title) || db.Pages.Any(p => p.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View("AddPage", pageViewModel);
                }

                //init pageDto
                var pageDto = new PageDto
                {
                    Title = pageViewModel.Title,
                    Slug = slug,
                    Body = pageViewModel.Body,
                    HasSidebar = pageViewModel.HasSidebar,
                    Sorting = 100
                };

                db.Pages.Add(pageDto);
                db.SaveChanges();
            }

            TempData["SM"] = "You have added a new page";

            return View();
        }

        // GET: Admin/Pages/EditPage/id
        public ActionResult EditPage(int id)
        {
            PageViewModel pageViewModel;
            using (Db db = new Db())
            {
                var pageDto = db.Pages.SingleOrDefault(p => p.Id == id);
                if (pageDto == null)
                    return Content("Page not exists.");
                pageViewModel = new PageViewModel(pageDto);
            }

            return View(pageViewModel);
        }

        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageViewModel pageViewModel)
        {
            if (!ModelState.IsValid)
                return View(pageViewModel);

            using (Db db = new Db())
            {
                var pageDto = db.Pages.SingleOrDefault(p => p.Id == pageViewModel.Id);
                if (pageDto == null)
                    return Content("Page not found.");

                string slug = "home";
                if (pageViewModel.Slug != "home")
                    slug = String.IsNullOrWhiteSpace(pageViewModel.Slug)
                    ? pageViewModel.Title.Replace(" ", "-").ToLower()
                    : pageViewModel.Slug.ToLower();

                //make sure title and slug are unique
                if (db.Pages
                    .Where(p => p.Id != pageViewModel.Id)
                    .Any(p => p.Title.ToLower() == pageViewModel.Title.ToLower() || p.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(pageViewModel);
                }

                //update the db
                pageDto.Title = pageViewModel.Title;
                pageDto.Body = pageViewModel.Body;
                pageDto.Slug = slug;
                pageDto.HasSidebar = pageViewModel.HasSidebar;
                db.SaveChanges();
            }
            TempData["SM"] = "Page has been updated.";
            return View(pageViewModel);
        }

        // POST: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                var pageDto = db.Pages.SingleOrDefault(p => p.Id == id);
                if (pageDto == null)
                    return Content("Page not exists.");
                if (pageDto.Slug == "home")
                    return Content("Home page cannot be delete");

                db.Pages.Remove(pageDto);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            PageViewModel pageViewModel;
            using (Db db = new Db())
            {
                var pageDto = db.Pages.SingleOrDefault(p => p.Id == id);
                if (pageDto == null)
                    return Content("Page not exists.");
                pageViewModel = new PageViewModel(pageDto);
            }

            return View(pageViewModel);
        }

        // POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (var db = new Db())
            {
                // Set initial count
                int count = 1;

                // Declare PageDto
                PageDto pageDto;
                // Set sorting for each page
                foreach (var pageId in id)
                {
                    pageDto = db.Pages.Find(pageId);
                    pageDto.Sorting = count;
                    db.SaveChanges();
                    count++;
                }
            }
        }

        // GET: Admin/Pages/EditSidebar
        public ActionResult EditSidebar()
        {
            SidebarViewModel sidebarViewModel;
            using (var db = new Db())
            {
                SidebarDto dto = db.Sidebar.Find(1);
                sidebarViewModel = new SidebarViewModel(dto);
            }
            return View(sidebarViewModel);
        }

        // POSt: Admin/Pages/EditSidebar

        [HttpPost]
        public ActionResult EditSidebar(SidebarViewModel sidebarViewModel)
        {
            using (var db = new Db())
            {
                SidebarDto dto = db.Sidebar.Find(1);

                dto.Body = sidebarViewModel.Body;
                db.SaveChanges();
            }
            TempData["SM"] = "You have edited the sidebar!";
            return RedirectToAction("EditSidebar");
        }
    }
}