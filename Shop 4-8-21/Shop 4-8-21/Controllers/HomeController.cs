using Shop_4_8_21.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Net.Mail;
using System.Net;

namespace Shop_4_8_21.Controllers
{
    public class HomeController : Controller
    {

        ShopEntities1 db = new ShopEntities1();



        public ActionResult Index()
        {
            List<tbl_product> SomeList = db.tbl_product.Where(model => model.featured == "yes").ToList();
            TempData["MyList"] = SomeList;
            List<tbl_product> ProductList = db.tbl_product.ToList();
            TempData["ProductList"] = ProductList;

            return View();
        }


        public ActionResult All_Products(int? Page_no, int? minPrice, int? maxPrice, string featured, string low_to_high, string high_to_low, string search,string Category)
        {

            if (Session["u_id"] == null)
            {
                return RedirectToAction("User_Login", "User");
            }
            else
            {
                Pro pag = new Pro();
                pag.Products = db.tbl_product.ToList();
                Page_no = Page_no.HasValue ? Page_no.Value > 0 ? Page_no.Value : 1 : 1;
                int Page = Page_no.Value;

                if (!string.IsNullOrEmpty(low_to_high))
                {
                    pag.Products = pag.Products.OrderBy(model => model.p_price).ToList();
                }
                if (!string.IsNullOrEmpty(high_to_low))
                {
                    pag.Products = pag.Products.OrderByDescending(model => model.p_price).ToList();
                }
                if (minPrice.HasValue && maxPrice.HasValue)
                {
                    pag.Products = pag.Products.Where(p => p.p_price >= minPrice && p.p_price <= maxPrice).ToList();

                }
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    pag.Products = pag.Products.Where(p => p.p_name.ToLower().Contains(search)).ToList();

                }

                if (!string.IsNullOrEmpty(Category))
                {
                    var c = db.tbl_category.Where(m => m.c_name == Category).SingleOrDefault();
                    pag.Products = pag.Products.Where(p => p.cat_fk == c.c_id).ToList();
                }

                TempData["ProductCount"] = pag.Products.Count();
                pag.Pager = new Pager(pag.Products.Count(), Page_no, 9);

                pag.Products = pag.Products.Skip((Page_no.Value - 1) * 9).Take(9).ToList();
                return View(pag);
            }

        }





        public ActionResult Product_View(int id)
        {


            if (Session["u_id"] == null)
            {
                return RedirectToAction("User_Login", "User");
            }
            else
            {
                var data = db.tbl_product.Where(m => m.p_id == id).SingleOrDefault();
                TempData["Related_Product"] = db.tbl_product.Where(o => o.cat_fk == data.cat_fk).ToList();
                TempData["Rating"] = db.tbl_rating.Where(x => x.pro_id_fk == id).ToList();
                Rating_Percentage(id);
                return View(data);

            }

        }


        [HttpPost]
        public ActionResult Buy_Now(int id, string qty)
        {

            if (Session["u_id"] == null)
            {
                return RedirectToAction("User_Login", "User");
            }
            else
            {

                if (id != 0)
                {
                    var data = db.tbl_product.Where(m => m.p_id == id).SingleOrDefault();
                    ViewBag.price = data.p_price * Convert.ToInt32(qty);
                    return View(data);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

        }


        public ActionResult Add_To_Cart()
        {

            if (Session["u_id"] == null)
            {
                return RedirectToAction("User_Login", "User");
            }
            else
            {
                Cart p = new Cart();

                var Cart = Request.Cookies["CartProducts"];

                if (Cart != null && !string.IsNullOrEmpty(Cart.Value))
                {
                    p.ProductId = Cart.Value.Split('-').Select(x => int.Parse(x)).ToList();
                    p.MyCart = db.tbl_product.Where(x => p.ProductId.Contains(x.p_id)).ToList();
                    return View(p);
                }
                else
                {
                    return View();
                }

            }

        }



        [HttpPost]
        public ActionResult Order()
        {
            if (Session["u_id"] == null)
            {
                return RedirectToAction("User_Login", "User");
            }
            else
            {
                var Cart = Request.Cookies["CartProducts"];
                tbl_order order = new tbl_order();

                List<string> files = new List<string>();
                List<string> qty = new List<string>();

                if (Cart != null && !string.IsNullOrEmpty(Cart.Value))
                {
                    var ProductIds = Cart.Value.Split('-').Select(x => int.Parse(x)).Distinct().ToList();
                    var Ids = Cart.Value.Split('-').Select(x => int.Parse(x)).ToList();

                    foreach (var item in ProductIds.ToList())
                    {
                        files.Add(item.ToString());
                        qty.Add(Ids.Where(x => x == item).Count().ToString());

                        ViewBag.pro = string.Join(",", files);
                        ViewBag.qty = string.Join(",", qty);
                    }


                    order.o_items = ViewBag.pro;
                    order.quantity = ViewBag.qty;
                    order.o_billing_address = Session["u_address"].ToString();
                    order.o_billing_name = Session["u_name"].ToString();
                    order.o_date = DateTime.Now;
                    order.o_payment = "COD";
                    order.o_status = "Processing";
                    order.o_total = Convert.ToInt32(TempData["Total"]);
                    order.u_fk = Convert.ToInt32(Session["u_id"]);

                    db.tbl_order.Add(order);
                    db.SaveChanges();
                    Send_Email(Session["u_email"].ToString(), Session["u_name"].ToString());


                }

                return RedirectToAction("Index", "Home");
            }

        }



        public ActionResult Favourite()
        {
            if (Session["u_id"] == null)
            {
                return RedirectToAction("User_Login", "User");
            }
            else
            {
                Favourite f = new Favourite();

                var fav = Request.Cookies["favProducts"];

                if (fav != null && !string.IsNullOrEmpty(fav.Value))
                {
                    f.ProductId = fav.Value.Split('-').Select(x => int.Parse(x)).ToList();
                    f.MyFav = db.tbl_product.Where(x => f.ProductId.Contains(x.p_id)).ToList();
                    return View(f);
                }
                else
                {
                    return View();
                }

            }

        }


        public ActionResult Your_Orders()
        {
            if (Session["u_id"] == null)
            {
                return RedirectToAction("User_Login", "User");
            }
            else
            {
                int id = Convert.ToInt32(Session["u_id"]);
                var data = db.tbl_order.Where(x => x.u_fk == id).ToList();
                return View(data);
            }

        }



        public ActionResult Contact()
        {
            return View();
        }




        public void Send_Email(string email, string name)
        {
            var senderemail = new MailAddress("xoale57@gmail.com", "Shopistic");
            var receivermail = new MailAddress(email, name);
            string pass = "azizabad";
            var sub = "Your Order Has Been Placed";
            //var body = "Thank you for ordering from Daraz! <br /> We're excited for you to receive your order {0} and will notify you once it's on its way. If you have ordered from multiple sellers, your items will be delivered in separate packages. We hope you had a great shopping experience!";
            var sntp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderemail.Address, pass)
            };

            using (var mess = new MailMessage(senderemail, receivermail)
            {
                Subject = sub,
                Body = "<h2>Thanks For Ordering With Us!</h2> <br /> <p>We're excited for you to receive your order and will notify you once it's on its way.  We hope you had a great shopping experience! </p> <br /> <a href='/Home/Your_Orders'>Shop More</a> ",
                IsBodyHtml = true
            }
            )
            {
                try
                {
                    sntp.Send(mess);
                }
                catch (Exception)
                {

                    HttpNotFound();
                }
               
            }
        }


       
        [HttpPost]
        public int Rating(string star,string msg,int? id)
        {
            tbl_rating r = new tbl_rating();

            if (star != null || msg != null || id != null)
            {
                r.comment = msg.ToString();
                r.rating = Convert.ToInt32(star);
                r.user_id_fk = Convert.ToInt32(Session["u_id"]);
                r.pro_id_fk = Convert.ToInt32(id);

                db.tbl_rating.Add(r);
                db.SaveChanges();
                return 1;
            }
            else
            {
                return 0;
            }


        }


        public void Rating_Percentage(int id)
        {
            var data = db.tbl_rating.Where(x => x.pro_id_fk == id).ToList();
            double i = Convert.ToDouble(data.Sum(x => x.rating));
            double c = data.Count();

            double total_rating = i / c;
            double per = total_rating / 5 * 100;
            TempData["Percentage"] = per;
        }




    }

}