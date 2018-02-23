using MVCShoppingCart.Models.Data;
using MVCShoppingCart.Models.ViewModels.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace MVCShoppingCart.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "Your cart is empty.";
                return View();
            }

            // Calculate total and save to ViewBag
            decimal totalPrice = 0m;
            foreach (var item in cart)
            {
                totalPrice += item.TotalPrice;
            }
            ViewBag.GrandTotal = totalPrice;

            return View(cart);
        }

        public ActionResult CartPartial()
        {
            // Init the CartViewModel
            CartViewModel cartViewModel = new CartViewModel();

            // Init the quantity
            int qty = 0;

            // Init price
            decimal price = 0m;

            // Check for cart session
            if (Session["cart"] != null)
            {
                // Get and set total qty and price
                var list = (List<CartViewModel>)Session["cart"];
                foreach (var item in list)
                {
                    qty += item.Quantity;
                    price = item.Quantity * item.ProductPrice;
                }

                cartViewModel.Quantity = qty;
                cartViewModel.ProductPrice = price;
            }
            else
            {
                // Or set qty and price to 0
                cartViewModel.Quantity = 0;
                cartViewModel.ProductPrice = 0m;
            }

            return PartialView(cartViewModel);
        }

        public ActionResult AddToCartPartial(int id)
        {
            // Init CartViewModel list
            List<CartViewModel> cart = Session["cart"] as List<CartViewModel> ??
                                       new List<CartViewModel>();

            // Init CartViewModel
            CartViewModel cartViewModel = new CartViewModel();

            using (Db db = new Db())
            {
                // Get the product
                var product = db.Products.First(p => p.Id == id);

                // Check if the product is already in cart
                var productInCart = cart.FirstOrDefault(c => c.ProductId == id);

                // If not, add new
                if (productInCart == null)
                {
                    cart.Add(new CartViewModel
                    {
                        ProductId = product.Id,
                        Image = product.ImageName,
                        ProductName = product.Name,
                        ProductPrice = product.Price,
                        Quantity = 1
                    });
                }
                // If it is, increment
                else
                {
                    productInCart.Quantity++;
                }
            }

            // Get total qty and price and add to CartVM
            int qty = 0;
            decimal price = 0m;
            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.ProductPrice;
            }

            cartViewModel.Quantity = qty;
            cartViewModel.ProductPrice = price;

            // Save cart back to session
            Session["cart"] = cart;

            return PartialView(cartViewModel);
        }

        // GET: /Cart/IncrementProduct?productId=
        public ActionResult IncrementProduct(int productId)
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Get CartVM from the list
            var cartViewModel = cart.First(c => c.ProductId == productId);

            // Increment the qty
            cartViewModel.Quantity++;

            // Store needed data

            //
            return Json(new { qty = cartViewModel.Quantity, price = cartViewModel.ProductPrice }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Cart/DecrementProduct?productId=
        public ActionResult DecrementProduct(int productId)
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Get CartVM from the list
            var cartViewModel = cart.First(c => c.ProductId == productId);

            // Decrement the qty
            if (cartViewModel.Quantity > 1)
                cartViewModel.Quantity--;
            else
            {
                cart.Remove(cartViewModel);
                cartViewModel.Quantity = 0;
            }

            // Store needed data

            //
            return Json(new { qty = cartViewModel.Quantity, price = cartViewModel.ProductPrice }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Cart/RemoveProduct?productId=
        public void RemoveProduct(int productId)
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Get CartVM from the list
            var cartViewModel = cart.First(c => c.ProductId == productId);

            // Remove from the cart
            cart.Remove(cartViewModel);

        }

        public ActionResult PaypalPartial()
        {
            var cart = Session["cart"] as List<CartViewModel>;
            return PartialView(cart);
        }


        // POST: /Cart/PlaceOrder
        [HttpPost]
        public void PlaceOrder()
        {
            // Check if logged in
            if (User.Identity == null)
                Redirect("/account/login");

            // Get cart list
            var cart = Session["cart"] as List<CartViewModel> ?? new List<CartViewModel>();

            // Get username
            string username = User.Identity.Name;

            // Declare orderId
            int orderId;

            using (Db db = new Db())
            {
                // Init OrderDto
                int userId = db.Users.First(u => u.Username == username).Id;
                var orderDto = new OrderDto()
                {
                    CreatedAt = DateTime.Now,
                    // Get user id
                    UserId = userId
                };

                // Add to OrderDto and save
                db.Orders.Add(orderDto);
                db.SaveChanges();

                // Get inserted id
                orderId = orderDto.OrderId;

                // Init OrderDetailsDTO
                OrderDetailsDto orderDetailsDto = new OrderDetailsDto();

                // Add to OrderDetailsDTO
                foreach (var item in cart)
                {
                    orderDetailsDto.OrderId = orderId;
                    orderDetailsDto.UserId = userId;
                    orderDetailsDto.ProductId = item.ProductId;
                    orderDetailsDto.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDto);
                    db.SaveChanges();
                }
            }

            // Email admin
            // Send to fake smtp testing server https://mailtrap.io/

            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("af889f100755e4", "aa926e848dc049"),
                EnableSsl = true
            };
            client.Send("from@example.com", "to@example.com", "New order", $"You have a new order. Order number {orderId}");

            // Reset session
            Session["cart"] = null;
        }
    }
}