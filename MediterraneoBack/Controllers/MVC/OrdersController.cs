using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using MediterraneoBack.Classes;
using MediterraneoBack.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MediterraneoBack.Controllers.MVC
{
    [Authorize(Roles ="User")]
    public class OrdersController : Controller
    {
        private MediterraneoContext db = new MediterraneoContext();

        [HttpPost]
        public ActionResult AddProduct(AddProductView view)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            
            if (ModelState.IsValid)
            {
                var orderDetailTmp = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name && odt.ProductId == view.ProductId).FirstOrDefault();
                if (orderDetailTmp == null)
                {
                    var product = db.Products.Find(view.ProductId);
                    orderDetailTmp = new OrderDetailTmp
                    {
                        Description = product.Description,
                        BarCode = product.BarCode,
                        Reference = product.Reference,
                        Price = product.Price,
                        ProductId = product.ProductId,
                        Quantity = view.Quantity,
                        TaxRate = product.Tax.Rate,
                        UserName = User.Identity.Name,
                    };

                    db.OrderDetailTmps.Add(orderDetailTmp);
                }
                else
                {
                    orderDetailTmp.Quantity += view.Quantity;
                    db.Entry(orderDetailTmp).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Create");

            }
             ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(user.CompanyId), "ProductId", "BarCode");
            return PartialView(view);

        }

        public ActionResult AddProduct()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(user.CompanyId, true), "ProductId", "BarCode");
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddProductDescr(AddProductDescrView view)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (ModelState.IsValid)
            {
                var orderDetailTmp = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name && odt.ProductId == view.ProductId).FirstOrDefault();
                if (orderDetailTmp == null)
                {
                    var product = db.Products.Find(view.ProductId);
                    orderDetailTmp = new OrderDetailTmp
                    {
                        Description = product.Description,
                        BarCode = product.BarCode,
                        Reference = product.Reference,
                        Price = product.Price,
                        ProductId = product.ProductId,
                        Quantity = view.Quantity,
                        TaxRate = product.Tax.Rate,
                        UserName = User.Identity.Name,
                    };

                    db.OrderDetailTmps.Add(orderDetailTmp);
                }
                else
                {
                    orderDetailTmp.Quantity += view.Quantity;
                    db.Entry(orderDetailTmp).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("Create");

            }
            ViewBag.ProductId = new SelectList(CombosHelper.GetProductsDescr(user.CompanyId), "ProductId", "Description");
            return PartialView(view);

        }


        public ActionResult AddProductDescr()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.ProductId = new SelectList(CombosHelper.GetProductsDescr(user.CompanyId, true), "ProductId", "Description");
            return PartialView();
        }

        public ActionResult DeleteProduct(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderDetailTmp = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name && odt.ProductId == id).FirstOrDefault();
            
            if (orderDetailTmp == null)
            {
                return HttpNotFound();
            }

            db.OrderDetailTmps.Remove(orderDetailTmp);
            db.SaveChanges();
            return RedirectToAction("Create");
        }


        // GET: Orders
        public ActionResult Index()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var orders = db.Orders.Where(o => o.CompanyId == user.CompanyId).Include(o => o.State);
            var ordd = db.OrderDetails.OrderByDescending(o => o.OrderId);            
            return View(orders.OrderByDescending(o => o.OrderId).ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            ViewBag.SalespersonId = new SelectList(CombosHelper.GetCustomers(user.CompanyId), "SalespersonId", "FullName");
            ViewBag.DiscountId = new SelectList(CombosHelper.GetDiscounts(user.CompanyId), "DiscountId", "Description");
            var view = new NewOrderView
            {
                Date = DateTime.Now,
                Details = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name).OrderByDescending(l => l.OrderDetailTmpId).ToList(),
            };
            return View(view);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( NewOrderView view, string confirm_value)
        {
            if (ModelState.IsValid)
            {
               
                var response = MovementsHelper.NewOrder(view, User.Identity.Name);
                if (response.IsSucces)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.AddRange(new DataColumn[6]{
                        new DataColumn("COD_BARRA"),
                        new DataColumn("REFERENCIA"),
                        new DataColumn("CANTIDAD"),
                        new DataColumn("DESCRIPCION"),                        
                        new DataColumn("PRECIO"),
                        new DataColumn("VALOR_BRUTO", typeof(decimal))
                    });

                    var ordd = db.OrderDetails.OrderByDescending(o => o.OrderId);

                    var qry = (
                        
                               from o in ordd
                               let topOrders = ordd.Take(1).Select(p => p.OrderId).Distinct()                               
                               where topOrders.Contains(o.OrderId)
                               select o).ToList();

                    foreach (var item in qry)
                    {
                        dt.Rows.Add(item.BarCode, item.Reference, item.Quantity ,item.Description, item.Price.ToString("#,##0").PadLeft(6), (item.Price * item.Quantity).ToString("#,##0").PadLeft(6));
                    }


                    //SendPDFEmail(dt, view);
                    //SendXLSEmail(dt, view); 

                    if (ViewBag.Status = true)
                    {                        
                        SendPDFEmail(dt, view);
                        ViewBag.Message = "LA ORDEN HA SIDO ENVIADA CON EXITO!";
                        
                    }
                    else
                    {
                        SendXLSEmail(dt, view);
                        ViewBag.Message = "SE HA ENVIADO LA ORDEN Y LA PROFORMA!";
                        
                    }
                    
                    
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            ViewBag.SalespersonId = new SelectList(CombosHelper.GetCustomers(user.CompanyId), "SalespersonId", "FullName");
            ViewBag.DiscountId = new SelectList(CombosHelper.GetDiscounts(user.CompanyId), "DiscountId", "Description");

            view.Details = db.OrderDetailTmps.Where(odt => odt.UserName == User.Identity.Name).ToList();
            


            return View(view);
        }


        private void SendXLSEmail(DataTable dt, NewOrderView view)
        {
            var cust_qry = (

                               from ord in db.Orders
                               join cust in db.Salespersons on ord.SalespersonId equals cust.SalespersonId
                               orderby ord.OrderId descending
                               select new { cust.FirstName, cust.LastName, cust.UserName, cust.Address, cust.Phone, cust.RNC }).FirstOrDefault();

            var OrderNo_qry = (

                               from ord in db.Orders
                               orderby ord.OrderId descending
                               select new { ord.OrderId }).FirstOrDefault();

            var Order_Disc = (

                                from ord in db.Orders
                                join disc in db.Discounts on ord.DiscountId equals disc.DiscountId
                                orderby ord.OrderId descending
                                select new { disc.Description, disc.DiscountRate }).FirstOrDefault();

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {

                    var customerName = (cust_qry.FirstName + " " + cust_qry.LastName).ToUpper();
                    var customerEmail = cust_qry.UserName;
                    var customerAdress = cust_qry.Address;
                    var customerPhone = cust_qry.Phone;
                    string rnc = cust_qry.RNC;
                    string discount = Order_Disc.Description;
                    var disc_rate = Order_Disc.DiscountRate;
                    var orderNo = OrderNo_qry.OrderId.ToString("00###");

                    var sumOfValues = dt.AsEnumerable().Sum(row => row.Field<decimal>("VALOR_BRUTO")).ToString("#,##0.00##");
                    var total_br = dt.AsEnumerable().Sum(row => row.Field<decimal>("VALOR_BRUTO"));
                    var descTotal = (Convert.ToDouble(total_br) * disc_rate).ToString("#,##0.00##");
                    var pre_itbis = (Convert.ToDouble(total_br) * disc_rate);
                    var itbis_total = ((Convert.ToDouble(total_br) - pre_itbis) * 0.18).ToString("#,##0.##");
                    var pre_neto = ((Convert.ToDouble(total_br) - pre_itbis) * 0.18);
                    var neto_total = ((Convert.ToDouble(total_br) - pre_itbis) + pre_neto).ToString("#,##0.##");

                    string filepath = Server.MapPath("~/Content/Logos/Logo_Med.png");
                    StringBuilder sb = new StringBuilder();

                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0'>");
                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append("<img src='" + filepath + "'width='140' height='100' align='center'/>");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    //sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b> MEDITERRANEO INTERNACIONAL SRL </b></td></tr>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>ORDEN DE PEDIDO</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b># ORDEN: </b>");
                    sb.Append(orderNo);
                    sb.Append("</td><td align='right'><b>FECHA: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>CLIENTE: </b>");
                    sb.Append(customerName);

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>VENDEDOR: </b>");
                    sb.Append(User.Identity.Name);

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>RNC: </b>");
                    sb.Append(rnc);
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;  ");
                    sb.Append("<b>CONDICION: </b>");
                    sb.Append("CREDITO");
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>TELEFONO: </b>");
                    sb.Append(customerPhone);
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("<b>DESC.%: </b>");
                    sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append(discount);
                    sb.Append("</th></tr>");


                    sb.Append("<tr>");
                    sb.Append("<th align='left'>");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("<b>TOTAL BRUTO RD$: </b>");
                    sb.Append("&nbsp;");
                    sb.Append(sumOfValues);
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'>");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("<b>DESCUENTO RD$: </b>");
                    sb.Append("&nbsp;&nbsp; &nbsp;");
                    sb.Append(descTotal);
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'>");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("<b>ITBIS RD$:</b>");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append(itbis_total);
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'>");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("<b>VALOR NETO RD$: </b>");
                    sb.Append("&nbsp; &nbsp;");
                    sb.Append(neto_total);
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>DIRECCION: </b>");
                    sb.Append(cust_qry.Address);

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>OBSERVACION: </b>");
                    sb.Append(view.Remarks);

                    sb.Append("</table>");
                    sb.Append("<br />");
                    sb.Append("<table width='100%' align='center'  border = '0'>");
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        sb.Append("<th align='center' width='90%' border = '1' > <font size ='2'> ");
                        sb.Append("<b>" + column.ColumnName + "</b>");
                        sb.Append("</th>");
                    }
                    sb.Append("</tr>");
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("<td align='center' width='90%' > <font size ='2'>");
                            sb.Append(row[column]);
                            sb.Append("</font></td>");
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    StringReader sr = new StringReader(sb.ToString());

                    //, 14f, 14f, 14f, 0f
                    Document pdfDoc = new Document(PageSize.A4, 2.5f, 2.5f, 10f, 1f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);

                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();
                        byte[] bytes = memoryStream.ToArray();
                        memoryStream.Close();
                        ////n.gomez@mediterraneo.com.do
                        //e.peralta@mediterraneo.com.do                        
                        MailMessage mm = new MailMessage("mediterraneoapp@gmail.com", "anthonyovalles@gmail.com");
                        //mm.To.Add(new MailAddress("v.ogando@mediterraneo.com.do"));
                        //mm.To.Add(new MailAddress(customerEmail));
                        //mm.To.Add(new MailAddress(User.Identity.Name));
                        mm.Subject = "Nueva Orden de Pedido No_" + orderNo;
                        mm.Body = "Adjunto esta la ORDEN realizada por: " + User.Identity.Name + ", Observaciones: " + view.Remarks + "<br /> GRACIAS POR PREFERIRNOS!!";
                        mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "OrdenDePedido_" + orderNo + ".pdf"));                        
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        NetworkCredential NetworkCred = new NetworkCredential();
                        NetworkCred.UserName = "mediterraneoapp@gmail.com";
                        NetworkCred.Password = "bayovanex0705";
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
            }
        }


        public static MemoryStream DataTableToExcelXlsx(DataTable dt, string sheetName)
        {
            MemoryStream Result = new MemoryStream();
            ExcelPackage pack = new ExcelPackage();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add(sheetName);

            int col = 1;
            int row = 1;
            foreach (DataRow rw in dt.Rows)
            {
                foreach (DataColumn cl in dt.Columns)
                {
                    if (rw[cl.ColumnName] != DBNull.Value)
                        ws.Cells[row, col].Value = rw[cl.ColumnName].ToString();
                    col++;
                }
                row++;
                col = 1;
            }

            pack.SaveAs(Result);
            return Result;
        }
        //private const string CsvContentType = "application/ms-excel";

        private void SendPDFEmail(DataTable dt, NewOrderView view)
        {

            var cust_qry = (

                               from ord in db.Orders
                               join cust in db.Salespersons on ord.SalespersonId equals cust.SalespersonId
                               orderby ord.OrderId descending
                               select new {cust.FirstName, cust.LastName, cust.UserName, cust.Address, cust.Phone, cust.RNC }).FirstOrDefault();

            var OrderNo_qry = (

                               from ord in db.Orders
                               orderby ord.OrderId descending
                               select new { ord.OrderId }).FirstOrDefault();

            var Order_Disc = (

                                from ord in db.Orders
                                join disc in db.Discounts on ord.DiscountId equals disc.DiscountId
                                orderby ord.OrderId descending
                                select new { disc.Description, disc.DiscountRate}).FirstOrDefault();

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    
                    var customerName = (cust_qry.FirstName+" "+ cust_qry.LastName).ToUpper();
                    var customerEmail = cust_qry.UserName;
                    var customerAdress = cust_qry.Address;
                    var customerPhone = cust_qry.Phone;
                    string rnc = cust_qry.RNC;
                    string discount = Order_Disc.Description;
                    var disc_rate = Order_Disc.DiscountRate;
                    var orderNo = OrderNo_qry.OrderId.ToString("00###");

                    var sumOfValues = dt.AsEnumerable().Sum(row => row.Field<decimal>("VALOR_BRUTO")).ToString("#,##0.00##");
                    var total_br = dt.AsEnumerable().Sum(row => row.Field<decimal>("VALOR_BRUTO"));
                    var descTotal = (Convert.ToDouble(total_br) * disc_rate).ToString("#,##0.00##");
                    var pre_itbis = (Convert.ToDouble(total_br) * disc_rate);
                    var itbis_total = ((Convert.ToDouble(total_br) - pre_itbis) * 0.18).ToString("#,##0.##");
                    var pre_neto = ((Convert.ToDouble(total_br) - pre_itbis) * 0.18);
                    var neto_total = ((Convert.ToDouble(total_br) - pre_itbis) + pre_neto).ToString("#,##0.##");

                    string filepath = Server.MapPath("~/Content/Logos/Logo_Med.png");
                    StringBuilder sb = new StringBuilder();

                    sb.Append("<table width='100%' cellspacing='0' cellpadding='0'>");
                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append("<img src='"+ filepath+ "'width='140' height='100' align='center'/>");
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    //sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b> MEDITERRANEO INTERNACIONAL SRL </b></td></tr>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>ORDEN DE PEDIDO</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b># ORDEN: </b>");
                    sb.Append(orderNo);
                    sb.Append("</td><td align='right'><b>FECHA: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>CLIENTE: </b>");
                    sb.Append(customerName);

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>VENDEDOR: </b>");
                    sb.Append(User.Identity.Name);

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>RNC: </b>");
                    sb.Append(rnc);
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;  ");
                    sb.Append("<b>CONDICION: </b>");                   
                    sb.Append("CREDITO");
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>TELEFONO: </b>");
                    sb.Append(customerPhone);
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("<b>DESC.%: </b>");
                    sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append(discount);                   
                    sb.Append("</th></tr>");


                    sb.Append("<tr>");
                    sb.Append("<th align='left'>");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("<b>TOTAL BRUTO RD$: </b>");
                    sb.Append("&nbsp;");
                    sb.Append(sumOfValues);
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'>");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("<b>DESCUENTO RD$: </b>");
                    sb.Append("&nbsp;&nbsp; &nbsp;");
                    sb.Append(descTotal);
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'>");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append("<b>ITBIS RD$:</b>");
                    sb.Append("&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;");
                    sb.Append(itbis_total);
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'>");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp;");
                    sb.Append("&nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; &nbsp;&nbsp; ");
                    sb.Append("<b>VALOR NETO RD$: </b>");
                    sb.Append("&nbsp; &nbsp;");
                    sb.Append(neto_total);
                    sb.Append("</th></tr>");

                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>DIRECCION: </b>");
                    sb.Append(cust_qry.Address);
                    
                    sb.Append("<tr>");
                    sb.Append("<th align='left'><b>OBSERVACION: </b>");
                    sb.Append(view.Remarks);

                    sb.Append("</table>");
                    sb.Append("<br />");
                    sb.Append("<table width='100%' align='center'  border = '0'>");
                    sb.Append("<tr>");
                    foreach (DataColumn column in dt.Columns)
                    {
                        sb.Append("<th align='center' width='90%' border = '1' > <font size ='2'> ");
                        sb.Append("<b>" + column.ColumnName + "</b>");
                        sb.Append("</th>");
                    }
                    sb.Append("</tr>");
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("<td align='center' width='90%' > <font size ='2'>"); 
                                 sb.Append(row[column]);
                            sb.Append("</font></td>");
                        }
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    StringReader sr = new StringReader(sb.ToString());

                    //, 14f, 14f, 14f, 0f
                    Document pdfDoc = new Document(PageSize.A4,2.5f,2.5f,10f,1f);                    
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream); 
                       
                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();
                        byte[] bytes = memoryStream.ToArray();
                        memoryStream.Close();
                        ////n.gomez@mediterraneo.com.do
                        //e.peralta@mediterraneo.com.do
                        MemoryStream ms = new MemoryStream();
                        ms = DataTableToExcelXlsx(dt, "PROFORMA_");
                        ms.Position = 0;
                        //MailMessage mm = new MailMessage("mediterraneoapp@gmail.com", "anthonyovalles@gmail.com");
                        ////mm.To.Add(new MailAddress("v.ogando@mediterraneo.com.do"));
                        ////mm.To.Add(new MailAddress(User.Identity.Name));                        
                        //mm.Subject = "Nueva Orden de Pedido No_" + orderNo;
                        //mm.Body = "Adjunto esta la ORDEN realizada por: " + User.Identity.Name + ", Observaciones: " + view.Remarks;
                        //mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "OrdenDePedido_" + orderNo + ".pdf"));

                        MailMessage clientmail = new MailMessage("mediterraneoapp@gmail.com", "anthonyovalles@gmail.com");
                        clientmail.Subject = "Nueva Orden de Pedido No_" + orderNo;
                        clientmail.Body = "Adjunto esta la ORDEN realizada por: " + User.Identity.Name + ", Observaciones: " + view.Remarks + "<br /> GRACIAS POR PREFERIRNOS!!!";
                        clientmail.Attachments.Add(new Attachment(new MemoryStream(bytes), "OrdenDePedido_" + orderNo + ".pdf"));
                        clientmail.Attachments.Add(new Attachment(ms, "PROFORMA_" + orderNo + ".xlsx"));
                        clientmail.IsBodyHtml = true;
                        //mm.IsBodyHtml = true;                        
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        NetworkCredential NetworkCred = new NetworkCredential();
                        NetworkCred.UserName = "mediterraneoapp@gmail.com";
                        NetworkCred.Password = "bayovanex0705";
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        //smtp.Send(mm);
                        smtp.Send(clientmail);
                    }   
                }
            }
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.StateId = new SelectList(db.States, "StateId", "Description", order.StateId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderId,SalespersonId,StateId,Date,Remarks")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StateId = new SelectList(db.States, "StateId", "Description", order.StateId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
