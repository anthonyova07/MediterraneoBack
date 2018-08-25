using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MediterraneoBack.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [Display(Name = "City")]
        public string Name { get; set; }


        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Regiones")]
        public int DepartmentId { get; set; }
        public IEnumerable<Department> Departments { get; set; }

        [JsonIgnore]
        public virtual Department Department { get; set; }

        [JsonIgnore]
        public virtual ICollection<Company> Companies { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<Warehouse> Warehouses { get; set; }

        [JsonIgnore]
        public virtual ICollection<Salesperson> Salespersons { get; set; }

        //[ManyToOne]
        //public Department Department { get; set; }

        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        //public List<Customer> Customers { get; set; }

        //public override int GetHashCode()
        //{
        //    return CityId;
        //}

    }
}