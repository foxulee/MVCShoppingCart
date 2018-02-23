using MVCShoppingCart.Models.Data;
using MVCShoppingCart.Models.ViewModels.Account;
using MVCShoppingCart.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCShoppingCart.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: /account/login
        public ActionResult Login()
        {
            // Confirm user if not logging in
            string username = User.Identity.Name;

            if (!String.IsNullOrEmpty(username))
                return RedirectToAction("user-profile");

            return View();
        }

        // POST /account/login
        [HttpPost]
        public ActionResult Login(LoginUserViewModel loginUserViewModel)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View(loginUserViewModel);
            }

            // Check if the user is valid

            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(u => u.Username == loginUserViewModel.Username && u.Password == loginUserViewModel.Password))
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                loginUserViewModel.Password = "";
                return View(loginUserViewModel);
            }

            FormsAuthentication.SetAuthCookie(loginUserViewModel.Username, loginUserViewModel.RememberMe);
            return Redirect(FormsAuthentication.GetRedirectUrl(loginUserViewModel.Username, loginUserViewModel.RememberMe));
        }

        // GET /account/logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        // GET /account/user-profile
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            // Get username
            string username = User.Identity.Name;


            // Declare UserProfileViewModel
            UserProfileViewModel userProfileViewModel;

            using (Db db = new Db())
            {
                // Get user
                var userDto = db.Users.FirstOrDefault(u => u.Username == username);

                // Build model
                userProfileViewModel = userDto == null ? new UserProfileViewModel() : new UserProfileViewModel(userDto);
            }

            return View("UserProfile", userProfileViewModel);
        }

        // GET /account/user-profile
        [ActionName("user-profile")]
        [Authorize]
        [HttpPost]
        public ActionResult UserProfile(UserProfileViewModel userProfileViewModel)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View("UserProfile", userProfileViewModel);
            }

            // Check if passwords match if need be
            if (!string.IsNullOrWhiteSpace(userProfileViewModel.Password))
            {
                if (!userProfileViewModel.Password.Equals(userProfileViewModel.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View("UserProfile", userProfileViewModel);
                }
            }

            using (Db db = new Db())
            {
                // Get username
                string username = User.Identity.Name;

                // Make sure username is unique
                if (db.Users.Where(x => x.Id != userProfileViewModel.Id).Any(x => x.Username == username))
                {
                    ModelState.AddModelError("", "Username " + userProfileViewModel.Username + " already exists.");
                    userProfileViewModel.Username = "";
                    return View("UserProfile", userProfileViewModel);
                }

                // Edit DTO
                var userDto = db.Users.Find(userProfileViewModel.Id);

                userDto.FirstName = userProfileViewModel.FirstName;
                userDto.LastName = userProfileViewModel.LastName;
                userDto.EmailAddress = userProfileViewModel.EmailAddress;
                userDto.Username = userProfileViewModel.Username;

                if (!string.IsNullOrWhiteSpace(userProfileViewModel.Password))
                {
                    userDto.Password = userProfileViewModel.Password;
                }

                // Save
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "You have edited your profile!";

            // Redirect
            return Redirect("~/account/user-profile");
        }

        // GET: /account/create-account
        [ActionName("create-account")]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: /account/create-account
        [HttpPost]
        [ActionName("create-account")]
        public ActionResult CreateAccount(UserViewModel userViewModel)
        {
            // Check model state
            if (!ModelState.IsValid)
                return View("CreateAccount", userViewModel);

            // Check if passwords match
            if (userViewModel.Password != userViewModel.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View("CreateAccount", userViewModel);
            }

            // Make sure username is unique
            using (Db db = new Db())
            {
                if (db.Users.Any(u => u.Username == userViewModel.Username))
                {
                    ModelState.AddModelError("", $"Username {userViewModel.Username} is taken.");
                    userViewModel.Username = "";
                    return View("CreateAccount", userViewModel);
                }

                // Create userDto
                var userDto = new UserDto
                {
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    Password = userViewModel.Password,
                    Username = userViewModel.Username,
                    EmailAddress = userViewModel.EmailAddress
                };

                // Add the DTO and save
                db.Users.Add(userDto);
                db.SaveChanges();

                // Add to userRoleDto and save
                db.UserRoles.Add(new UserRoleDto
                {
                    RoleId = db.Roles.First(r => r.Name == "User").Id,
                    UserId = userDto.Id
                });
                db.SaveChanges();
            }

            // Create TempData message
            TempData["SM"] = "You are now registered and can login.";

            return View("Login");
        }

        [Authorize]
        public ActionResult UserNavPartial()
        {
            // Get username
            string username = User.Identity.Name;

            // Declare userPartialViewModel
            UserNavPartialViewModel userNavPartialViewModel;

            using (Db db = new Db())
            {
                // Get the user
                var userDto = db.Users.FirstOrDefault(u => u.Username == username);

                // Build the view model
                userNavPartialViewModel = userDto == null ? new UserNavPartialViewModel() : new UserNavPartialViewModel
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName
                };
            }

            return PartialView(userNavPartialViewModel);
        }

        [Authorize(Roles = "User")]
        public ActionResult Orders()
        {
            // Init list of OrdersForUserViewModel
            List<OrdersForUserViewModel> ordersForUserViewModelList = new List<OrdersForUserViewModel>();

            using (Db db = new Db())
            {
                // Get user id
                int userId = db.Users.First(u => u.Username == User.Identity.Name).Id;

                // Init list of OrderViewModel
                List<OrderViewModel> orderViewModelList = db.Orders
                    .Where(o => o.UserId == userId)
                    .ToList()
                    .Select(o => new OrderViewModel(o))
                    .ToList();

                foreach (var orderViewModel in orderViewModelList)
                {
                    // Init products dict
                    Dictionary<string, int> productAndQtyDict = new Dictionary<string, int>();

                    // Declare total
                    decimal total = 0m;

                    // Loop through OrderDetailsDto
                    foreach (var orderDetailsDto in db.OrderDetails.Where(o => o.OrderId == orderViewModel.OrderId).ToList())
                    {
                        // Get product info
                        var productDto = db.Products.First(p => p.Id == orderDetailsDto.ProductId);
                        decimal productPrice = productDto.Price;
                        string productName = productDto.Name;

                        // Add to product dict
                        productAndQtyDict.Add(productName, orderDetailsDto.Quantity);

                        //Get total
                        total += productPrice * orderDetailsDto.Quantity;
                    }

                    // Add to OrdersForUserVM list
                    ordersForUserViewModelList.Add(new OrdersForUserViewModel()
                    {
                        CreatedAt = orderViewModel.CreatedAt,
                        OrderNumber = orderViewModel.OrderId,
                        ProductsAndQty = productAndQtyDict,
                        Total = total
                    });
                }

            }

            return View(ordersForUserViewModelList);
        }
    }
}