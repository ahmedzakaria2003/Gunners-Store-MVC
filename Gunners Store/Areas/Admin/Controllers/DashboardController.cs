using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models.ViewModels;
using Gunner.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Gunners_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(IUnitOfWork unitOfWork , UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var dashboardData = new DashboardVM
            {
                Categories = _unitOfWork.CategoryReporistory.GetAll().OrderBy(c => c.DisplayOrder).Take(5).ToList(),
                Products = _unitOfWork.ProductReporistory.GetAll(includeProperties: "Category")
                                .OrderByDescending(p => p.ProductID)
                                .Take(8)
                                .ToList(),
             
                OrderDetails = _unitOfWork.orderDetail.GetAll(includeProperties: "Product,OrderHeader")
                                .Take(10)
                                .ToList(),

                TotalCategories = _unitOfWork.CategoryReporistory.GetAll().Count(),
                TotalProducts = _unitOfWork.ProductReporistory.GetAll().Count(),
                TotalOrders = _unitOfWork.orderHeader.GetAll().Count(),
                UserCount = _userManager.Users.Count()

            };

            return View(dashboardData);
        }

        public IActionResult GetOrderStatusData()
        {
            var statusData = new
            {
                labels = new[] { "Pending", "Approved", "In Process", "Completed", "Cancelled" },
                data = new[]
                {
            _unitOfWork.orderHeader.Count(o => o.OrderStatus == SD.StatusPending),
            _unitOfWork.orderHeader.Count(o => o.OrderStatus == SD.StatusApproved),
            _unitOfWork.orderHeader.Count(o => o.OrderStatus == SD.StatusInProcess),
            _unitOfWork.orderHeader.Count(o => o.OrderStatus == SD.StatusShipped),
            _unitOfWork.orderHeader.Count(o => o.OrderStatus == SD.StatusCancelled)
        }
            };
            return Json(statusData);
        }

    
public IActionResult GetMonthlyRevenueData()
        {
            try
            {
                var monthlyRevenue = _unitOfWork.orderHeader
                    .GetAll(
                        o => o.OrderDate >= DateTime.Now.AddMonths(-6),
                        includeProperties: "OrderDetails" 
                    )
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new
                    {
                        Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                        Revenue = g.SelectMany(o => o.OrderDetails).Sum(od => od.Price * od.Count)
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                return Json(new
                {
                    labels = monthlyRevenue.Select(x => x.Month).ToArray(),
                    data = monthlyRevenue.Select(x => x.Revenue).ToArray()
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred while processing your request." });
            }
        }
    }
    }

