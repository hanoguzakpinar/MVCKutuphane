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
    public class DilController : Controller
    {
        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();
        private bool isInsert = false;
        private bool isUpdate = false;
        private bool isDelete = false;

        // GET: Dil
        [Authorize]
        public ActionResult Index(string Isim, int? page)
        {
            if (string.IsNullOrEmpty(Isim))
            {
                var dataList = db.Dil.ToList().OrderBy(x => x.Isim).ToPagedList(page ?? 1, 10);
                return View(dataList);
            }

            if (string.IsNullOrEmpty(Isim)) Isim = "";

            var dataListFilter = db.Dil.Where(x => x.Isim.Contains(Isim)).OrderBy(x => x.Isim).ToPagedList(page ?? 1, 10);
            return View(dataListFilter);
        }

        [HttpGet]
        public ActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(Dil dil)
        {
            if (isInsert)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(dil.Isim))
            {
                ModelState.AddModelError("Isim", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                isInsert = true;

                try
                {
                    int data = db.Dil.Where(i => i.Isim == dil.Isim).Count();
                    if (data != 0)
                    {
                        isInsert = false;
                        return Json(data: new { success = 3, message = "DİL SİSTEMDE KAYITLI!" }, JsonRequestBehavior.AllowGet);
                    }


                    db.Dil.Add(dil);
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
            var datas = db.Dil.Where(x => x.ID == ID).SingleOrDefault();
            if (datas != null)
            {
                if (datas.Kitap.Count != 0)
                {
                    isDelete = false;
                    return Json(data: new { success = 3, message = "KAYITLI KİTAP BULUNUYOR!", title = "SİLİNEMEZ" }, JsonRequestBehavior.AllowGet);
                }

                db.Dil.Remove(datas);
                db.SaveChanges();

                isDelete = false;
                return Json(data: new { success = 0, message = "SİLME İŞLEMİ BAŞARILI!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(data: new { success = -1, message = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Update(int ID)
        {
            var data = db.Dil.Where(item => item.ID == ID).Single();
            if (data != null)
            {
                return View("Update", data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Update(Dil dil)
        {
            if (isUpdate)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(dil.Isim))
            {
                ModelState.AddModelError("Isim", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                isUpdate = true;

                try
                {
                    int data = db.Dil.Where(i => i.Isim == dil.Isim && i.ID != dil.ID).Count();
                    if (data != 0)
                    {
                        isUpdate = false;
                        return Json(data: new { success = 3, message = "DİL SİSTEMDE KAYITLI!" }, JsonRequestBehavior.AllowGet);
                    }


                    var _data = db.Dil.Where(item => item.ID == dil.ID).SingleOrDefault();
                    _data.Isim = dil.Isim;

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