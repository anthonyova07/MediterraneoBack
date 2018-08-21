using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace MediterraneoBack.Models
{
    public class User
    {

        [Key]
        public int UserId { get; set; }


        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(2556, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [Display(Name = "E-Mail")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(20, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(100, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        public string Address { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Photo { get; set; }

        
        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "City")]
        public int? CityId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        
        [Display(Name = "User")]
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        [NotMapped]
        public HttpPostedFileBase PhotoFile { get; set; }

        public virtual Department Department { get; set; }

        public virtual City City { get; set; }

        public virtual Company Company { get; set; }

        //public virtual Category Category { get; set; }

        public IEnumerable<Department> Departments { get; set; }

        public IEnumerable<City> Cities { get; set; }

        public IEnumerable<Company> Companies { get; set; }


    }
}