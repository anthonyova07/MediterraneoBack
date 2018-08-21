using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MediterraneoBack.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }


        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [Display(Name = "Company")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(20, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        public string Address { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Logo { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        public int CityId { get; set; }

        [NotMapped]
        public HttpPostedFileBase LogoFile { get; set; }

        public virtual Department Department { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<User> Users { get; set; }

        //public virtual Category Category { get; set; }

        public IEnumerable<Department> Departments { get; set; }

        public IEnumerable<City> Cities { get; set; }

        

        //public virtual ICollection<Category> Categories { get; set; }


        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        //public List<User> Users { get; set; }

        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        //public List<Product> Products { get; set; }

        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        //public List<CompanyCustomer> CompanyCustomers { get; set; }

        //public override int GetHashCode()
        //{
        //    return CompanyId;
        //}

    }
}