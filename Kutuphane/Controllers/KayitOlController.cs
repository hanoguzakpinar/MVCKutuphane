using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kutuphane.Models.Entity;

namespace Kutuphane.Controllers
{
    public class KayitOlController : Controller
    {
        // GET: KayitOl
        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();

        [HttpGet]
        public ActionResult Kayit()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Kayit(Users p)
        {        
            if(!ModelState.IsValid)
            {
                return View();
            }
            int data = db.Users.Where(i => i.Tc == p.Tc).Count();
            if (data != 0)
            {
                return Json(data: new { success = 3, message = "TC NO SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
            }

            data = db.Users.Where(i => i.Email == p.Email).Count();
            if (data != 0)
            {
                return Json(data: new { success = 3, message = "EMAİL SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
            }
            data = db.Users.Where(i => i.Username == p.Username).Count();
            if (data != 0)
            {
                return Json(data: new { success = 3, message = "KULLANICI ADI NUMARASI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
            }
            int data2 = p.Password.Length;
            if (data2 < 6)
            {
                return Json(data: new { success = 3, message = "Şifre en az 6 haneli olmalıdır." }, JsonRequestBehavior.AllowGet);
            }
            DateTime localDate = DateTime.Now;
            p.Yetki = 0;
            p.UyelikTarihi = localDate;
            p.DogumYeri = "Güncelle";
            p.Cinsiyet = "0";
            p.DogumTarihi = localDate;
            db.Users.Add(p);
            db.SaveChanges();
            return RedirectToAction("GirisYap", "Login");
        }

    }
}