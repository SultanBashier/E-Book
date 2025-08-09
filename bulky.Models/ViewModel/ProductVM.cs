using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.Models.ViewModel
{
    public class ProductVM
    {
        // Represents the product entity with all its properties
        public Product Product { get; set; }

        // Provides a list of categories for use in dropdowns (e.g., for selecting a product category in a form)
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryListItems { get; set; }    
    }
}
