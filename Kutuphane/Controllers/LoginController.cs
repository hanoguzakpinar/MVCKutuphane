using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kutuphane.Models.Entity;
using System.Web.Security;

namespace Kutuphane.Controllers
{
    public class LoginController : Controller
    {
        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();
        // GET: Login
        public ActionResult GirisYap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GirisYap(Users p)
        {
            var bilgiler = db.Users.FirstOrDefault(x => x.Email == p.Email && x.Password == p.Password);
            if (bilgiler != null)
            {
                FormsAuthentication.SetAuthCookie(bilgiler.Email, false);
                Session["Email"] = bilgiler.Email.ToString();

                //TempData["TC"] = bilgiler.Tc.ToString();
                Session["İsim"] = bilgiler.Adi.ToString();
                Session["Soyisim"] = bilgiler.Soyadi.ToString();
                //TempData["DTarihi"] = bilgiler.DogumTarihi.ToString();
                //TempData["DYeri"] = bilgiler.DogumYeri.ToString();
                //TempData["Telefon"] = bilgiler.Telefon.ToString();
                //TempData["UTarihi"] = bilgiler.UyelikTarihi.ToString();
                //TempData["Cinsiyet"] = bilgiler.Cinsiyet.ToString();
                //TempData["Adres"] = bilgiler.Adres.ToString();
                //TempData["Kadi"] = bilgiler.Username.ToString();
                //TempData["Sifre"] = bilgiler.Password.ToString();
                //TempData["Email"] = bilgiler.Email.ToString();

                if (bilgiler.Yetki == 1 || bilgiler.Yetki == 2)
                {
                    return RedirectToAction("Index", "EmanetVer");
                }
                else
                {
                    return RedirectToAction("Index", "Panel");
                }

            }
            else
            {
                ViewBag.Mesaj = "Hatalı Giriş";
                return View();
            }
        }
    }
}