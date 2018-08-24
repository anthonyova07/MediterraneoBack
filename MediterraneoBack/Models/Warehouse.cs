﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MediterraneoBack.Models
{
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        [Index("Warehouse_CompanyId_Name_Index", 1, IsUnique = true)]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }


        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [Display(Name = "Warehouse")]
        [Index("Warehouse_CompanyId_Name_Index", 2, IsUnique = true)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(20, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(100, ErrorMessage = "The field {0} must be maximum {1} characters length")]
        public string Address { get; set; }


        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "Regiones")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a {0}")]
        [Display(Name = "City")]
        public int CityId { get; set; }

        public virtual Department Department { get; set; }

        public virtual City City { get; set; }

        public virtual Company Company { get; set; }

        public virtual ICollection<Inventory> Inventories { get; set; }


    }
}