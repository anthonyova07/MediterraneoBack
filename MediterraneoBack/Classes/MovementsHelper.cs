using MediterraneoBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MediterraneoBack.Classes
{
    public class MovementsHelper : IDisposable
    {
        private static MediterraneoContext db = new MediterraneoContext();


        //Para cerrar la base de datos inmediatamente deje de ser utilizada
        public void Dispose()
        {
            db.Dispose();
        }

        public static Response NewOrder(NewOrderView view, string userName)
        {
            using (var transacction = db.Database.BeginTransaction())
            {
                try
                {
                    var user = db.Users.Where(u => u.UserName == userName).FirstOrDefault();
                    

                    var order = new Order
                    {
                        CompanyId = user.CompanyId,
                        SalespersonId = view.SalespersonId,
                        DiscountId = view.DiscountId,
                        
                        Date = view.Date,                        
                        Remarks = view.Remarks,                        
                        StateId = DBHelper.GetState("Created", db),
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();
                    var details = db.OrderDetailTmps.Where(odt => odt.UserName == userName).ToList();

                    foreach (var detail in details)
                    {
                        var orderDetail = new OrderDetail
                        {
                            Description = detail.Description,
                            BarCode = detail.BarCode,
                            Reference = detail.Reference,
                            OrderId = order.OrderId,                            
                            Price = detail.Price,
                            ProductId = detail.ProductId,
                            Quantity = detail.Quantity,
                            TaxRate = detail.TaxRate,
                        };

                        db.OrderDetails.Add(orderDetail);
                        db.OrderDetailTmps.Remove(detail);
                    }

                    db.SaveChanges();
                    transacction.Commit();
                    return new Response { IsSucces = true, };
                }
                catch (Exception ex)
                {
                    //Me mantiene la integridad de la bd en caso de que una transaccion no se complete correctamente
                    transacction.Rollback();
                    return new Response
                    {
                        Message = ex.Message,
                        IsSucces = false,
                    };
                }
            }
        }
    }
}