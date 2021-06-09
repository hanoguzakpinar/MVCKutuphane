using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Kutuphane.Models;
using Kutuphane.Models.Entity;

namespace Kutuphane.Controllers
{
    public class PanelController : Controller
    {
        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();
        // GET: Panel

        [Authorize]
        public ActionResult Index()
        {
            var uyemail = (String)Session["Email"];
            var degerler = db.Users.FirstOrDefault(z => z.Email == uyemail);
            return View(degerler);
        }

        public ActionResult Cikis()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("GirisYap", "Login");
        }
        [HttpPost]
        public ActionResult Index2(Users p)
        {
            int data = p.Telefon.Length;
            if (data != 11)
            {
                return Json(data: new { success = 3, message = "Telefon numarası 11 haneli olmalıdır." }, JsonRequestBehavior.AllowGet);
            }
            int data2 = p.Password.Length;
            if (data2 <6)
            {
                return Json(data: new { success = 3, message = "Şifre en az 6 haneli olmalıdır." }, JsonRequestBehavior.AllowGet);
            }

            var kullanici = (String)Session["Email"];
            var uye = db.Users.FirstOrDefault(x => x.Email == kullanici);
            uye.Password = p.Password;
            uye.Telefon = p.Telefon;
            uye.Adres = p.Adres;
            uye.Username = p.Username;
            db.SaveChanges();
            FormsAuthentication.SignOut();
            return RedirectToAction("GirisYap", "Login");
        }

        public ActionResult Kitaplarim()
        {
            var kullanici = (String)Session["Email"];
            var id = db.Users.Where(x => x.Email == kullanici.ToString()).Select(z => z.ID).FirstOrDefault();
            var degerler = db.ViewEmanetKitaplar.Where(x => x.KullaniciId == id).ToList();
            return View(degerler);
        }
    }
}