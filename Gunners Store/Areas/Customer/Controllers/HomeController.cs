using Microsoft.AspNetCore.Mvc;
using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models.Models;
using Gunners_Store.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace Gunners_Store.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        // GET: /Customer/Home/Index
        public IActionResult Index(int? page)
        {
            int pageSize = 8; // Number of products per page
            int pageNumber = (page ?? 1);

            var products = _unitOfWork.ProductReporistory.GetAll(includeProperties: "Category").AsQueryable();
            var paginatedProducts = PaginatedList<Product>.Create(products, pageNumber, pageSize);

            return View(paginatedProducts);
        }

        // GET: /Customer/Home/Details
        public IActionResult Details(int id)
        {
            var product = _unitOfWork.ProductReporistory.Get(u => u.ProductID == id, includeProperties: "Category");

            if (product == null)
            {
                return NotFound();
            }

            shoppinCart shoppingCart = new()
            {
                Product = product,
                Count = 1,
                ProductId = id
            };

            return View(shoppingCart);
        }

        // POST: /Customer/Home/Details
        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(shoppinCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;

            shoppinCart shopcart = _unitOfWork.ShoppingCartReporistory
                .Get(u => u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);

            if (shopcart != null)
            {
                shopcart.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCartReporistory.Update(shopcart);
            }
            else
            {
                _unitOfWork.ShoppingCartReporistory.Add(shoppingCart);
            }

            _unitOfWork.Save();
            TempData["success"] = "Product added to cart successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}