using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCShoppingCart.Models.Data;
using MVCShoppingCart.Models.ViewModels.Pages;

namespace MVCShoppingCart.Controllers
{
    public class PagesController : Controller
    {
        // GET: /{page}
        public ActionResult Index(string slug = "")
        {
            // Get/set page slug
            if (slug == "")
                slug = "home";

            // Declare PageViewModel and PageDto
            PageViewModel pageViewModel;
            PageDto pageDto;

            // Check if page exists
            using (Db db = new Db())
            {
                if (!db.Pages.Any(p => p.Slug == slug))
                    return RedirectToAction("Index", new { slug = "" });
            }

            // Get pageDto
            using (Db db = new Db())
            {
                pageDto = db.Pages.FirstOrDefault(p => p.Slug == slug);
            }

            // Set page title
            ViewBag.PageTitle = pageDto.Slug;

            // Check for sidebar
            ViewBag.Sidebar = pageDto.HasSidebar == true ? "Yes" : "No";

            // Init model
            pageViewModel = new PageViewModel(pageDto);

            return View(pageViewModel);
        }

        public ActionResult PagesMenuPartial()
        {
            // Declare a list of PageViewModel
            List<PageViewModel> pageVMList;

            // Get all pages except home
            using (Db db = new Db())
            {
                pageVMList = db.Pages
                    .Where(p => p.Slug != "home")
                    .OrderBy(p => p.Sorting).ToList()
                    .Select(p => new PageViewModel(p))
                    .ToList();
            }

            return PartialView(pageVMList);
        }

        public ActionResult SidebarPartial()
        {
            SidebarViewModel sidebarViewModel;
            using (Db db = new Db())
            {
                 sidebarViewModel = new SidebarViewModel(db.Sidebar.Find(1));
            }

            return PartialView(sidebarViewModel);
        }
    }
}