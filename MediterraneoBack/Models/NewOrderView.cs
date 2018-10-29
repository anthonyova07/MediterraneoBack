using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MediterraneoBack.Models
{
    public class NewOrderView
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Customer")]
        public int SalespersonId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }        

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Discount")]
        public int? DiscountId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Condition")]
        public int ConditionId { get; set; }

        public List<OrderDetailTmp> Details { get; set; }
            
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public int TotalQuantity { get { return Details == null ? 0 : Details.Sum(d => d.Quantity); } }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public double TotalBruto { get { return Details == null ? 0 : Convert.ToDouble( Details.Sum(d => d.Value)); } }
              
        
        //[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        //public double TotalDiscount { get { return Details == null ? 0 :Convert.ToDouble(TotalBruto) * Convert.ToDouble( DiscountId) ; } }

        //[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        //public double TotalItbis { get { return Details == null ? 0 : (Convert.ToDouble(TotalBruto) - Convert.ToDouble(TotalDiscount))* 0.18; } }

        //[DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        //public double TotalNeto { get { return Details == null ? 0 : (Convert.ToDouble(TotalBruto) - Convert.ToDouble(TotalDiscount)) + TotalItbis; } }






    }
}