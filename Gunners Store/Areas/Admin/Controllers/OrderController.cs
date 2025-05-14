using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models.Models;
using Gunner.Models.ViewModels;
using Gunner.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Climate;
using System.Web.Mvc;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using HttpGetAttribute = System.Web.Mvc.HttpGetAttribute;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace Gunners_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.orderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails(OrderVM OrderVM)  // Add parameter here
        {
            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(u => u.Id == OrderVM.orderHeader.Id);

            if (orderHeaderFromDb == null)
            {
                return NotFound();
            }

            // Update all fields
            orderHeaderFromDb.Name = OrderVM.orderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.orderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.orderHeader.City;
            orderHeaderFromDb.State = OrderVM.orderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.orderHeader.PostalCode;
            orderHeaderFromDb.ShippingDate = OrderVM.orderHeader.ShippingDate;
            orderHeaderFromDb.PaymentStatus = OrderVM.orderHeader.PaymentStatus;

            if (!string.IsNullOrEmpty(OrderVM.orderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.orderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.orderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            }

            _unitOfWork.orderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { OrderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        public IActionResult ShipOrder(OrderVM orderVM) 
        {
            if (orderVM?.orderHeader == null)
            {
                return NotFound();
            }

            var orderHeaderFromDb = _unitOfWork.orderHeader.Get(u => u.Id == orderVM.orderHeader.Id);

            if (orderHeaderFromDb == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(orderVM.orderHeader.TrackingNumber))
            {
                TempData["Error"] = "Tracking number is required";
                return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
            }

            if (string.IsNullOrEmpty(orderVM.orderHeader.Carrier))
            {
                TempData["Error"] = "Carrier is required";
                return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
            }

            orderHeaderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
            orderHeaderFromDb.Carrier = orderVM.orderHeader.Carrier;
            orderHeaderFromDb.OrderStatus = SD.StatusShipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;

            

            _unitOfWork.orderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
        }

        [HttpPost]
        public IActionResult StartProcessing(OrderVM orderVM)       
        {
            if (orderVM?.orderHeader == null)
            {
                return NotFound();
            }

            _unitOfWork.orderHeader.UpdateStatus(orderVM.orderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();

            TempData["Success"] = "Order Status Updated to In Process Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
        }



        [HttpPost]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            if (orderVM?.orderHeader == null)
            {
                TempData["Error"] = "Order data is missing";
                return RedirectToAction(nameof(Index));
            }

            var orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == orderVM.orderHeader.Id);

            if (orderHeader == null)
            {
                TempData["Error"] = "Order not found";
                return RedirectToAction(nameof(Index));
            }

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                service.Create(options); 
                _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }

            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.orderHeader.Id });
        }








        #region API Calls

        [HttpGet]
        public IActionResult GetAll(string status)
        {

            IEnumerable<OrderHeader> objOrderHeader = _unitOfWork.orderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {

                case "pending":
                    objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == SD.StatusPending);
                    break;

                case "inprocess":
                    objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == SD.StatusInProcess);    
                    break;
                case "completed":
                    objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeader = objOrderHeader.Where(u => u.OrderStatus == SD.StatusApproved);

                    break;
                default:
                    break;
            }

            return Json(new { data = objOrderHeader });

        }
        #endregion

    }
}