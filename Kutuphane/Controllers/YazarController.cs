using Kutuphane.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Kutuphane.Controllers
{
    public class YazarController : Controller
    {
        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();
        private bool isInsert = false;
        private bool isUpdate = false;
        private bool isDelete = false;

        // GET: Yazar
        [Authorize]
        public ActionResult Index(string Isim, string Soyisim, string Aciklama, int? page)
        {
            if (string.IsNullOrEmpty(Isim) && string.IsNullOrEmpty(Soyisim) && string.IsNullOrEmpty(Aciklama))
            {
                var dataList = db.Yazar.ToList().OrderBy(x => x.Isim).ThenBy(x => x.Soyisim).ToPagedList(page ?? 1, 10);
                return View(dataList);
            }

            if (string.IsNullOrEmpty(Isim)) Isim = "";
            if (string.IsNullOrEmpty(Soyisim)) Soyisim = "";
            if (string.IsNullOrEmpty(Aciklama)) Aciklama = "";

            var dataListFilter = db.Yazar.Where(x => x.Isim.Contains(Isim) && x.Soyisim.Contains(Soyisim) && (x.Aciklama.Contains(Aciklama) || x.Aciklama == null))
                .OrderBy(x => x.Isim).ThenBy(x => x.Soyisim).ToPagedList(page ?? 1, 10);
            return View(dataListFilter);
        }

        [HttpGet]
        public ActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(Yazar yazar)
        {
            if (isInsert)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(yazar.Isim))
            {
                ModelState.AddModelError("Isim", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(yazar.Soyisim))
            {
                ModelState.AddModelError("Soyisim", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                isInsert = true;

                try
                {
                    int data = db.Yazar.Where(i => i.Isim == yazar.Isim && i.Soyisim == yazar.Soyisim && i.Aciklama == yazar.Aciklama).Count();
                    if (data != 0)
                    {
                        isInsert = false;
                        return Json(data: new { success = 3, message = "YAZAR SİSTEMDE KAYITLI!" }, JsonRequestBehavior.AllowGet);
                    }


                    db.Yazar.Add(yazar);
                    db.SaveChanges();

                    isInsert = false;
                    return Json(data: new { success = 0, message = "KAYIT BAŞARILI!" }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e) { isInsert = false; }
            }
            else
            {
                return Json(data: new { success = 3, message = "LÜTFEN HATALARI DÜZELTİNİZ!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(data: new { success = 1, message = "BİR HATA OLDU!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int ID)
        {
            if (isDelete)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            isDelete = true;
            var datas = db.Yazar.Where(x => x.ID == ID).SingleOrDefault();
            if (datas != null)
            {
                if (datas.KitapYazarlari.Count != 0)
                {
                    isDelete = false;
                    return Json(data: new { success = 3, message = "KAYITLI KİTAP BULUNUYOR!", title = "SİLİNEMEZ" }, JsonRequestBehavior.AllowGet);
                }
                
                db.Yazar.Remove(datas);
                db.SaveChanges();

                isDelete = false;
                return Json(data: new { success = 0, message = "SİLME İŞLEMİ BAŞARILI!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(data: new { success = -1, message = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Update(int ID)
        {
            var data = db.Yazar.Where(item => item.ID == ID).Single();
            if (data != null)
            {
                return View("Update", data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Update(Yazar yazar)
        {
            if (isUpdate)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(yazar.Isim))
            {
                ModelState.AddModelError("Isim", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(yazar.Soyisim))
            {
                ModelState.AddModelError("Soyisim", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                isUpdate = true;

                try
                {
                    int data = db.Yazar.Where(i => i.Isim == yazar.Isim && i.Soyisim == yazar.Soyisim && i.Aciklama == yazar.Aciklama && i.ID != yazar.ID).Count();
                    if (data != 0)
                    {
                        isUpdate = false;
                        return Json(data: new { success = 3, message = "YAZAR SİSTEMDE KAYITLI!" }, JsonRequestBehavior.AllowGet);
                    }


                    var ktg = db.Yazar.Where(item => item.ID == yazar.ID).SingleOrDefault();
                    ktg.Isim = yazar.Isim;
                    ktg.Soyisim = yazar.Soyisim;
                    ktg.Aciklama = yazar.Aciklama;

                    db.SaveChanges();

                    isUpdate = false;
                    return Json(data: new { success = 0, message = "BİLGİLERİ GÜNCELLENDİ!" }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception e) { isUpdate = false; }
            }
            else
            {
                return Json(data: new { success = 3, message = "LÜTFEN HATALARI DÜZELTİNİZ!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(data: new { success = 1, message = "BİR HATA OLDU!" }, JsonRequestBehavior.AllowGet);
        }
    }
}