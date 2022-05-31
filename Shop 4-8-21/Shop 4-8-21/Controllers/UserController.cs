using Shop_4_8_21.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop_4_8_21.Controllers
{
    public class UserController : Controller
    {

        ShopEntities1 db = new ShopEntities1();

        public ActionResult User_Signup()
        {
            return View();
        }



        [HttpPost]
        public ActionResult User_Signup(tbl_user u)
        {
            if (u != null)
            {
                db.tbl_user.Add(u);
                db.SaveChanges();
                ModelState.Clear();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.Clear();
                return View();
            }
        }





        public ActionResult User_Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult User_Login(tbl_user l)
        {
            if (l != null)
            {
                var data = db.tbl_user.Where(m => m.u_email == l.u_email && m.u_password == l.u_password).FirstOrDefault();
                if (data != null)
                {
                    Session["u_id"] = data.u_id.ToString();
                    Session["u_name"] = data.u_name.ToString();
                    Session["u_address"] = data.u_address.ToString();
                    Session["u_contact"] = data.u_contact.ToString();
                    Session["u_email"] = data.u_email.ToString();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
                
            }
            else
            {
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult User_Profile(int id)
        {
            if (Session["u_id"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var data = db.tbl_user.Where(m => m.u_id == id).SingleOrDefault();

                return View(data);
            }
            
        }


        [HttpPost]
        public int User_Profile(tbl_user u)
        {
            if (u != null)
            {
                db.Entry(u).State = EntityState.Modified;
                db.SaveChanges();
                return 1;
            }
            else
            {
                return 0;
            }

        }


        [HttpPost]
        public int Update_Password(string old_p, string new_p, int id)
        {

            if (old_p != null && new_p != null)
            {
                var find = db.tbl_user.Where(u => u.u_id == id).SingleOrDefault();

                if (find.u_password == old_p)
                {
                    find.u_address = find.u_address;
                    find.u_city = find.u_city;
                    find.u_contact = find.u_contact;
                    find.u_email = find.u_email;
                    find.u_id = id;
                    find.u_name = find.u_name;
                    find.u_password = new_p.ToString();
                    db.Entry(find).State = EntityState.Modified;
                    db.SaveChanges();
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                return 0;
            }
        }



        [HttpPost]
        public int Forget_Password(string email, string password)
        {
            if (email != null)
            {
                var find = db.tbl_user.Where(x => x.u_email == email).SingleOrDefault();

                if (find.u_email == email)
                {
                    if (password != null)
                    {
                        find.u_address = find.u_address;
                        find.u_city = find.u_city;
                        find.u_contact = find.u_contact;
                        find.u_email = find.u_email;
                        find.u_id = find.u_id;
                        find.u_name = find.u_name;
                        find.u_password = password.ToString();
                        db.Entry(find).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                

                return 1;
            }
            else
            {
                return 0;
            }

        }



    }
}