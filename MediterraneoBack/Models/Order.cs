using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MediterraneoBack.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0} between {1} and {2}")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Customer")]
        public int SalespersonId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "State")]
        public int StateId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Discount")]
        public double DiscountId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Condition")]
        public int? ConditionId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(72, ErrorMessage = "The filed {0} must be maximun {1} characters length")]
        public string Remarks { get; set; }        

        public List<OrderDetailTmp> Details { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalValue { get { return Details == null ? 0 : Details.Sum(d => d.Value); } }

        [JsonIgnore]
        public virtual Salesperson Salesperson { get; set; }

        [JsonIgnore]
        public virtual State State { get; set; }

        [JsonIgnore]
        public virtual Company Company { get; set; }

        [JsonIgnore]
        public virtual Discount Discount { get; set; }

        [JsonIgnore]
        public virtual Condition Condition { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        [JsonIgnore]
        public virtual ICollection<OrderDetailTmp> OrderDetailTmps { get; set; }

      
    }
}