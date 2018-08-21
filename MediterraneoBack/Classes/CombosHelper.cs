using MediterraneoBack.Models;
using System;
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
                Name = "Select a Department",
            });

            return departments.OrderBy(d => d.Name).ToList();

        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}