using Microsoft.AspNetCore.Mvc.Rendering;

namespace BP_Document_Generation.ViewModels {
    public class DocumentCustomerSelectViewModel {
        public int SelectedCustomerId { get; set; }
        public IEnumerable<SelectListItem> Customers { get; set; }
    }
}
