using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kutuphane.Models.Entity;
using PagedList;
using PagedList.Mvc;


namespace Kutuphane.Controllers
{
    public class HomeController : Controller
    {

        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Kitaplar(int sayfa = 1)
        {
            var degerler = db.Kitap.ToList().ToPagedList(sayfa, 8);

            return View(degerler);
        }

        [HttpPost]
        public ActionResult Kitaplar(string Adi, int sayfa=1)
        {
            var degerler = db.Kitap.Where(x=>x.Adi.Contains(Adi)).ToList().ToPagedList(sayfa, 8);

            return View(degerler);
        }

        public ActionResult Detay(int ID)
        {
            
            List<Yazar> yazarList = (from i in db.Yazar
                                     join ky in db.KitapYazarlari on i.ID equals ky.YazarId
                                     where ky.KitapId == ID
                                     orderby i.Isim, i.Soyisim
                                     select i).ToList();

            if (yazarList != null)
            {
                string yazars = "";
                for (int i = 0; i < yazarList.Count; i++)
                {
                    string ns = yazarList[i].Isim + ' ' + yazarList[i].Soyisim;
                    yazars += ns;
                    if (i != yazarList.Count - 1)
                        yazars += ", ";
                }

                ViewBag.yazars = yazars;
            }

            Kategori kategori = (from kat in db.Kategori
                                 join kit in db.Kitap on kat.ID equals kit.DeweyKod
                                 where kit.ID == ID
                                 select kat).Single();

            if (kategori != null)
            {
                ViewBag.deweyName = kategori.DeweyId + " - " + kategori.Isim;
            }


            var data = db.Kitap.Where(item => item.ID == ID).Single();

            ViewBag.emaneteUygunmu = "Uygun Değil";
            if (data.EmaneteUygunmu == true)
            {
                ViewBag.emaneteUygunmu = "Uygun";
            }

            if (data != null)
            {
                return View("Detay", data);
            }

            return View();
        }
    }
}