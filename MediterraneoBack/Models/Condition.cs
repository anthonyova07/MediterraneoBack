using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MediterraneoBack.Models
{
    public class Condition
    {
        [Key]
        public int ConditionId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [Display(Name = "Condition")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}