using Shop_4_8_21.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop_4_8_21.Controllers
{
    public class AdminController : Controller
    {

        ShopEntities1 db = new ShopEntities1();

        public ActionResult Dashboard()
        {
            if (Session["Admin_id"] == null)
            {
                return RedirectToAction("Admin_login", "Admin");
            }
            else
            {
                TempData["Product_Count"] = db.tbl_product.Count();
                TempData["User_Count"] = db.tbl_user.Count();
                TempData["Order_Count"] = db.tbl_order.Count();
                TempData["Order_Total"] = db.tbl_order.Sum(p =>p.o_total);

                return View();
            }
        }


        public ActionResult Admin_Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Admin_Login(tbl_admin a)
        {
            var data = db.tbl_admin.Where(model => model.a_email == a.a_email && model.a_password == a.a_password).FirstOrDefault();
            if (data != null)
            {
                Session["Admin_id"] = data.a_id.ToString();
                Session["Admin_name"] = data.a_name.ToString();
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                ModelState.Clear();
                return View();
            }
        }



        public ActionResult All_Category()
        {

            if (Session["Admin_id"] == null)
            {
                return RedirectToAction("Admin_login", "Admin");
            }
            else
            {
                var data = db.tbl_category.ToList();
                return PartialView(data);
            }

        }



        public ActionResult Category_delete(int id)
        {
            var data = db.tbl_category.Where(m => m.c_id == id).SingleOrDefault();
            db.Entry(data).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("All_Category", "Admin");
        }


        public ActionResult Category()
        {
            if (Session["Admin_id"] == null)
            {
                return RedirectToAction("Admin_login", "Admin");
            }
            else
            {
                return View();
            }

        }



        [HttpPost]
        public int Cat(string cat_name)
        {

            if (cat_name != null)
            {
                tbl_category c = new tbl_category();
                c.c_name = cat_name;

                db.tbl_category.Add(c);
                db.SaveChanges();
                return 1;
            }
            else
            {
                return 0;
            }
        }


        public ActionResult All_Product()
        {
            if (Session["Admin_id"] == null)
            {
                return RedirectToAction("Admin_login", "Admin");
            }
            else
            {
                var data = db.tbl_product.ToList();
                return PartialView(data);
            }

        }



        [HttpPost]
        public ActionResult Product_delete(int id)
        {
            var data = db.tbl_product.Where(m => m.p_id == id).SingleOrDefault();
            string image = Request.MapPath(data.images.ToString());
            if (System.IO.File.Exists(image))
            {
                System.IO.File.Delete(image);
            }
            db.Entry(data).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("All_Product", "Admin");
        }

        public ActionResult Product_Edit(int id)
        {
            var data = db.tbl_product.Where(x => x.p_id == id).FirstOrDefault();
            TempData["Images"] = data.images.ToString();
            TempData.Keep();
            return PartialView(data);
        }

        [HttpPost]
        public ActionResult Product_Edit(tbl_product v)
        {
            

            if (v.ImageFile != null)
            {
                string filename = Path.GetFileNameWithoutExtension(v.ImageFile.FileName);
                string extension = Path.GetExtension(v.ImageFile.FileName);

                if (extension == ".jpg" || extension == ".png" || extension == ".jpeg")
                {
                    filename = filename + extension;
                    v.images = "~/Images/" + filename;
                    filename = Path.Combine(Server.MapPath("~/Images/"), filename);
                    v.ImageFile.SaveAs(filename);

                    db.Entry(v).State = EntityState.Modified;
                    db.SaveChanges();

                    string image = Request.MapPath(TempData["Images"].ToString());
                    if (System.IO.File.Exists(image))
                    {
                        System.IO.File.Delete(image);
                    }

                    ModelState.Clear();
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    ModelState.Clear();
                    return RedirectToAction("Dashboard", "Admin");
                }
            }

            else
            {
                v.images = TempData["Images"].ToString();
                db.Entry(v).State = EntityState.Modified;
                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("Dashboard", "Admin");
            }

            
        }



        public ActionResult Product()
        {

            if (Session["Admin_id"] == null)
            {
                return RedirectToAction("Admin_login", "Admin");
            }
            else
            {
                List<tbl_category> p = db.tbl_category.ToList();
                ViewBag.cat = new SelectList(p, "c_id", "c_name");

                return PartialView();
            }

        }



        [HttpPost]
        public ActionResult Product(tbl_product p)
        {
            string filename = Path.GetFileNameWithoutExtension(p.ImageFile.FileName);
            string extension = Path.GetExtension(p.ImageFile.FileName);

            if (extension == ".jpg" || extension == ".png" || extension == ".jpeg")
            {
                filename = filename + extension;
                p.images = "~/Images/" + filename;
                filename = Path.Combine(Server.MapPath("~/Images/"), filename);
                p.ImageFile.SaveAs(filename);

                p.featured = "no";
                db.tbl_product.Add(p);
                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                ModelState.Clear();
                //return PartialView();
                return Content("<script language='javascript' type='text/javascript'>alert('Error');</script>");
            }
        }




        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Admin_login", "Admin");
        }




        public ActionResult All_Customers()
        {
            if (Session["Admin_id"] == null)
            {
                return RedirectToAction("Admin_login", "Admin");
            }
            else
            {
                var data = db.tbl_user.ToList();
                return PartialView(data);
            }

        }


        [HttpPost]
        public ActionResult Customer_delete(int id)
        {
            var data = db.tbl_user.Where(m => m.u_id == id).SingleOrDefault();
            db.Entry(data).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("All_Customers", "Admin");
        }



        public ActionResult Order()
        {

            if (Session["Admin_id"] == null)
            {
                return RedirectToAction("Admin_login", "Admin");
            }
            else
            {
                var data = db.tbl_order.OrderByDescending(x => x.o_id).ToList();
                return PartialView(data);
            }

        }



        [HttpPost]
        public ActionResult Order_Update(string status,int? id)
        {
            if (status != null && id != null)
            {
                var data = db.tbl_order.Where(x => x.o_id == id).SingleOrDefault();

                data.o_id = Convert.ToInt32(id);
                data.o_billing_address = data.o_billing_address;
                data.o_billing_name = data.o_billing_name;
                data.o_date = data.o_date;
                data.o_items = data.o_items;
                data.o_payment = data.o_payment;
                data.o_status = status;
                data.o_total = data.o_total;
                data.quantity = data.quantity;
                data.u_fk = data.u_fk;

                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Order","Admin");

            }
            else
            {
                return RedirectToAction("Order", "Admin");
            }
        }






    }
}