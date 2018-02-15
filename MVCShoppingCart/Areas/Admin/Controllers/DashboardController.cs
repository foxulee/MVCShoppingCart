using System.Web.Mvc;

namespace MVCShoppingCart.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        public ActionResult Index()
        {

            return View();
        }
    }
}