using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.Models.ViewModel
{
    public class RoleManagmentVM
    {

        // Represents the application user whose role or company is being managed
        public ApllicationUser apllicationUser { get; set; }


        // List of available roles for selection in the UI (e.g., dropdown)
        public IEnumerable<SelectListItem> RoleList { get; set; }


        // List of available companies for selection in the UI (e.g., dropdown)
        public IEnumerable<SelectListItem> CompanyList { get; set; }
    }
}
