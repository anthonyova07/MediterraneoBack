using MediterraneoBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediterraneoBack.Classes
{
    public class CombosHelper : IDisposable
    {
        private static MediterraneoContext db = new MediterraneoContext();

        public static List<Department> GetDepartments()
        {
            var departments = db.Departments.ToList();
            departments.Add(new Department
            {
                DepartmentId = 0,
                Name = "[Select a Region...]",
            });

            return departments.OrderBy(d => d.Name).ToList();

        }


        public static List<Product> GetProducts(int companyId, bool sw)
        {
            var products = db.Products.Where(p => p.CompanyId == companyId).ToList();
            return products.OrderBy(p => p.Description).ToList();
        }


        public static List<Product> GetProducts(int companyId)
        {
            var products = db.Products.Where(p => p.CompanyId == companyId).ToList();
            products.Add(new Product
            {
                ProductId = 0,
                Description = "[Select a product...]",
            });
            return products.OrderBy(p => p.BarCode).ToList();

        }

        public static List<Company> GetCompanies()
        {
            var companies = db.Companies.ToList();
            companies.Add(new Company
            {
                CompanyId = 0,
                Name = "[Select a company...]",
            });
            return companies.OrderBy(d => d.Name).ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public static List<Category> GetCategories(int companyId)
        {
            var categories = db.Categories.Where(c => c.CompanyId == companyId).ToList();
            categories.Add(new Category
            {
                CategoryId = 0,
                Description = "[Select a category...]",
            });
            return categories.OrderBy(d => d.Description).ToList();
        }

        public static List<Tax> GetTaxes(int companyId)
        {
            var taxes = db.Taxes.Where(c => c.CompanyId == companyId).ToList();
            taxes.Add(new Tax
            {
                TaxId = 0,
                Description = "[Select a tax...]",
            });
            return taxes.OrderBy(d => d.Description).ToList();
        }

        public static List<Discount> GetDiscounts(int companyId)
        {
            var discounts = db.Discounts.Where(c => c.CompanyId == companyId).ToList();
            discounts.Add(new Discount
            {
                DiscountId = 0,
                Description = "[Select a Discount...]",
            });
            return discounts.OrderBy(d => d.Description).ToList();
        }

        public static List<City> GetCities()
        {
            var cities = db.Cities.ToList();
            cities.Add(new City
            {
                CityId = 0,
                Name = "[Select a city...]",
            });
            return cities.OrderBy(d => d.Name).ToList();
        }

        public static List<Salesperson> GetCustomers(int companyId)
        {

            var qry = (from cu in db.Salespersons
                       join cc in db.CompanyCustomers on cu.SalespersonId equals cc.SalespersonId
                       join co in db.Companies on cc.CompanyId equals co.CompanyId
                       where co.CompanyId == companyId
                       select new { cu }).ToList();
            
            var salespersons = new List<Salesperson>();
            foreach (var item in qry)
            {
                salespersons.Add(item.cu);
            }

            salespersons.Add(new Salesperson
            {
                SalespersonId = 0,
                FirstName = "[Select a Customer...]",
            });
            return salespersons.OrderBy(d => d.FirstName).ThenBy(c => c.LastName).ToList();
        }

        
    }
}