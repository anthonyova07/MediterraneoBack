using MediterraneoBack.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                Name = "[Select a Region]",
            });

            return departments.OrderBy(d => d.Name).ToList();

        }

        internal static List<Product> GetProducts(int companyId)
        {
            var products = db.Products.Where(p => p.CompanyId == companyId).ToList();
            products.Add(new Product
            {
                ProductId = 0,
                Description = "[Select a product...]",
            });
            return products.OrderBy(p => p.Description).ToList();

        }

        public static List<Company> GetCompanies()
        {
            var companies = db.Companies.ToList();
            companies.Add(new Company
            {
                CompanyId = 0,
                Name = "[Select a company]",
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
                Description = "[Select a category]",
            });
            return categories.OrderBy(d => d.Description).ToList();
        }

        public static List<Tax> GetTaxes(int companyId)
        {
            var taxes = db.Taxes.Where(c => c.CompanyId == companyId).ToList();
            taxes.Add(new Tax
            {
                TaxId = 0,
                Description = "[Select a tax]",
            });
            return taxes.OrderBy(d => d.Description).ToList();
        }

        public static List<City> GetCities()
        {
            var cities = db.Cities.ToList();
            cities.Add(new City
            {
                CityId = 0,
                Name = "[Select a city]",
            });
            return cities.OrderBy(d => d.Name).ToList();
        }

        public static List<Salesperson> GetCustomers(int companyId)
        {
            var salespersons = db.Salespersons.Where(c => c.CompanyId == companyId).ToList();
            salespersons.Add(new Salesperson
            {
                SalespersonId = 0,
                FirstName = "[Select a Client]",
            });
            return salespersons.OrderBy(d => d.FirstName).ThenBy(c => c.LastName).ToList();
        }
    }
}