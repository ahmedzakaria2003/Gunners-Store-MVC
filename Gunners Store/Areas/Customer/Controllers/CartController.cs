using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models.Models;
using Gunner.Models.ViewModels;
using Gunner.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using System.Security.Claims;
using System.Web.Mvc;
using ActionNameAttribute = Microsoft.AspNetCore.Mvc.ActionNameAttribute;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;
using Stripe.Checkout;
using SessionService = Stripe.Checkout.SessionService;
using Session = Stripe.Checkout.Session;

namespace Gunners_Store.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]

    public class CartController : Controller

    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User Not Found");
            }

            var userId = userIdClaim.Value;
            shoppingCartVM = new ShoppingCartVM()
            {
                shoppingCartList = _unitOfWork.ShoppingCartReporistory.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
            };

            foreach (var cart in shoppingCartVM.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }

        //[ActionName("Summary")]
        [HttpPost]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.shoppingCartList = _unitOfWork.ShoppingCartReporistory.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            if (shoppingCartVM.orderHeader == null)
            {
                shoppingCartVM.orderHeader = new OrderHeader();
            }
            shoppingCartVM.orderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.orderHeader.ApplicationUserId = userId;

            var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            if (applicationUser == null)
            {
                return NotFound("المستخدم غير موجود.");
            }

            shoppingCartVM.orderHeader.ApplicationUserId = userId; 
            shoppingCartVM.orderHeader.Name = applicationUser.Name;
            shoppingCartVM.orderHeader.State = applicationUser.State;
            shoppingCartVM.orderHeader.City = applicationUser.City;
            shoppingCartVM.orderHeader.StreetAddress = applicationUser.StreetAddress;
            shoppingCartVM.orderHeader.PostalCode = applicationUser.PostalCode;
            shoppingCartVM.orderHeader.PhoneNumber = applicationUser.PhoneNumber ?? "Not Provided";

            shoppingCartVM.OrderTotal = 0;
            foreach (var cart in shoppingCartVM.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderTotal += (cart.Price * cart.Count);
            }

            shoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;

            _unitOfWork.orderHeader.Add(shoppingCartVM.orderHeader);
            _unitOfWork.Save();

            foreach (var cart in shoppingCartVM.shoppingCartList)
            {
                OrderDetail orderDetails = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.orderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.orderDetail.Add(orderDetails);
            }
            _unitOfWork.Save();

            var domain = "https://localhost:7026/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={shoppingCartVM.orderHeader.Id}",
                CancelUrl = domain + $"Customer/Cart/Index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var cart in shoppingCartVM.shoppingCartList)
            {
                var lineItem = new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Product.ProductName,
                        },
                        UnitAmountDecimal = (decimal)(cart.Price * 100),
                    },
                    Quantity = cart.Count,
                };
                options.LineItems.Add(lineItem);
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            _unitOfWork.orderHeader.UpdateStripePaymentId(shoppingCartVM.orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult OrderConfirmation(int id)
        {
            Console.WriteLine($"Received id: {id}");
            OrderHeader orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(orderHeader.SessionId))
            {
                return BadRequest();
            }

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.orderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.orderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            List<shoppinCart> shoppingCartList = _unitOfWork.ShoppingCartReporistory.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCartReporistory.RemoveRange(shoppingCartList);
            _unitOfWork.Save();

            return View(id);
        }

        public IActionResult Summary()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartVM = new ShoppingCartVM()
            {
                shoppingCartList = _unitOfWork.ShoppingCartReporistory.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                //OrderTotal = 0
                orderHeader = new()
            };

            shoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            shoppingCartVM.orderHeader.Name = shoppingCartVM.orderHeader.ApplicationUser.Name;
            shoppingCartVM.orderHeader.State = shoppingCartVM.orderHeader.ApplicationUser.State;
            shoppingCartVM.orderHeader.City = shoppingCartVM.orderHeader.ApplicationUser.City;
            shoppingCartVM.orderHeader.StreetAddress = shoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.orderHeader.PostalCode = shoppingCartVM.orderHeader.ApplicationUser.PostalCode;





            foreach (var cart in shoppingCartVM.shoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);

                shoppingCartVM.OrderTotal += (cart.Price * cart.Count);

            }

            return View(shoppingCartVM);
        }

        public IActionResult Plus(int id)
        {
            var CartFromDb = _unitOfWork.ShoppingCartReporistory.Get(u => u.Id == id);

            CartFromDb.Count += 1;
            _unitOfWork.ShoppingCartReporistory.Update(CartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int id)
        {
            var CartFromDb = _unitOfWork.ShoppingCartReporistory.Get(u => u.Id == id);

            if (CartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCartReporistory.Remove(CartFromDb);
            }
            else
            {
                CartFromDb.Count -= 1;
                _unitOfWork.ShoppingCartReporistory.Update(CartFromDb);
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int id)
        {
            var CartFromDb = _unitOfWork.ShoppingCartReporistory.Get(u => u.Id == id);

            _unitOfWork.ShoppingCartReporistory.Remove(CartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        private double GetPriceBasedOnQuantity(shoppinCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }

                else
                {
                    return shoppingCart.Product.Price100;
                }

            }

        }

    }
}

