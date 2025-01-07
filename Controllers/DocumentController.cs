using BP_Document_Generation.Models;
using BP_Document_Generation.Services.Interfaces;
using BP_Document_Generation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BP_Document_Generation.Controllers {
    public class DocumentController : Controller {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;

        public DocumentController(
            ICustomerService customerService,
            IProductService productService,
            IOrderService orderService,
            IOrderDetailService orderDetailService) {
            _customerService = customerService;
            _productService = productService;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<ActionResult> Index() {
            var customers = await _customerService.GetAllCustomersAsync();

            var customerItems = customers.Select(c => new SelectListItem {
                Value = c.CustomerID.ToString(),
                Text = string.Format("{0} {1}", c.FirstName, c.LastName)
            }).ToList();

            var viewModel = new DocumentCustomerSelectViewModel {
                Customers = customerItems
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateDocument(int selectedCustomerId) {

            var viewModel = await GetViewModel(selectedCustomerId);

            return StatusCode(StatusCodes.Status501NotImplemented, "This feature is not implemented yet.");
        }

        private async Task<OrderConfirmationViewModel?> GetViewModel(int customerId) {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null) {
                return null;
            }

            var order = await _orderService.GetOrderByCustomerIdAsync(customerId);
            if (order == null) {
                return null;
            }

            var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(order.OrderID);

            List<OrderItemViewModel> OrderItems = new List<OrderItemViewModel>();
            foreach (var orderDetail in orderDetails) {
                var product = await _productService.GetProductByIdAsync(orderDetail.ProductID);
                if (product != null) {
                    OrderItemViewModel item = new OrderItemViewModel {
                        ProductID = product.ProductID,
                        ProductName = product.Name,
                        Quantity = orderDetail.Quantity,
                        UnitPrice = product.UnitPrice,
                        TotalPrice = orderDetail.Quantity * product.UnitPrice
                    };
                    OrderItems.Add(item);
                }
            }

            var totalPrice = OrderItems.Sum(i => i.TotalPrice);

            return new OrderConfirmationViewModel {
                CustomerID = customer.CustomerID,
                CustomerName = string.Format("{0} {1}", customer.LastName, customer.FirstName),
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                TotalPrice = totalPrice,
                OrderItems = OrderItems
            };
        }
    }
}
