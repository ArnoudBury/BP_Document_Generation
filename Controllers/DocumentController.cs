using BP_Document_Generation.Models;
using BP_Document_Generation.Services.Interfaces;
using BP_Document_Generation.ViewModels;
using DocumentGeneration.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Telerik.Reporting;

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

            var documentBytes = GenerateTelerikDocument(viewModel);

            return File(documentBytes, "application/pdf", "OrderConfirmation.pdf");
        }

        private byte[] GenerateTelerikDocument(OrderConfirmationViewModel viewModel) {

            // Initialize the report parameters
            var reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();
            var deviceInfo = new System.Collections.Hashtable();

            // Unpackage the report
            Report report = null;
            using (var sourceStream = System.IO.File.OpenRead("Reports/OrderConfirmation.trdp")) {
                var reportPackager = new ReportPackager();
                report = (Report)reportPackager.UnpackageDocument(sourceStream);
            }

            // Create an ObjectDataSource for the report
            var objectDataSource = new ObjectDataSource {
                DataSource = viewModel
            };

            // Set the report's data source
            report.DataSource = objectDataSource;

            var instanceReportSource = new InstanceReportSource {
                ReportDocument = report
            };

            // Render the report to a byte array (PDF format)
            var result = reportProcessor.RenderReport("PDF", instanceReportSource, deviceInfo);

            return result.DocumentBytes;
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
                CustomerName = string.Format("{0} {1}", customer.FirstName, customer.LastName),
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
