using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.Models
{
    public class ApllicationUser:IdentityUser
    {
        //This Property of The User
        [Required]
        public string Name { get; set; }
        public string? StreetAdrress { get; set; }
        public string? City { get; set; }
        public string? state { get; set; }
        public string? Postalcode { get; set; }

        public int? CompanyId  { get; set; }
        [ForeignKey("CompanyId")]
        [ValidateNever]
        public Company? Company { get; set; }
        [NotMapped]
        public string Role { get; set; }
    }
}
