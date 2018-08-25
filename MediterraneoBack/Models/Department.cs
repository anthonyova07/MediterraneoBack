using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MediterraneoBack.Models
{
    public class Department
    {

        [Key]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage ="The field {0} is required")]
        [MaxLength(50, ErrorMessage ="The field {0} must be maximum {1} characters length")]
        [Display (Name = "Regiones")]
        [Index("Department_Name_Index", IsUnique = true)]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<City> Cities { get; set; }

        [JsonIgnore]
        public virtual ICollection<Company> Companies { get; set; }

        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }

        [JsonIgnore]
        public virtual ICollection<Warehouse> Warehouses { get; set; }

        [JsonIgnore]
        public virtual ICollection<Salesperson> Salespersons { get; set; }



        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        //public List<City> Cities { get; set; }

        //[OneToMany(CascadeOperations = CascadeOperation.All)]
        //public List<Customer> Customers { get; set; }


    }
}