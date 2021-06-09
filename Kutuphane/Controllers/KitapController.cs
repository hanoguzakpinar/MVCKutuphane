using Kutuphane.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Kutuphane.Models;

namespace Kutuphane.Controllers
{
    public class KitapController : Controller
    {
        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();
        private bool isInsert = false;
        private bool isUpdate = false;
        private bool isDelete = false;

        //[HttpGet]
        [Authorize]
        public ActionResult Index(string ISBN, string Adi, string[] filterDeweyIds, string[] filterYazarIds, string[] filterYayineviIds, string[] filterDilIds, int? page)
        {
            List<SelectListItem> katList = (from i in db.Kategori.ToList()
                                            orderby i.DeweyId
                                            select new SelectListItem
                                            {
                                                Text = i.DeweyId + " - " + i.Isim,
                                                Value = i.ID.ToString()
                                            }).ToList();
            ViewBag.katList = katList;

            List<SelectListItem> yazarList = (from i in db.Yazar.ToList()
                                              orderby i.Isim, i.Soyisim
                                              select new SelectListItem
                                              {
                                                  Text = i.Isim + " " + i.Soyisim,
                                                  Value = i.ID.ToString()
                                              }).ToList();
            ViewBag.yazarList = yazarList;

            List<SelectListItem> yayineviList = (from i in db.YayinEvi.ToList()
                                                 orderby i.Isim
                                                 select new SelectListItem
                                                 {
                                                     Text = i.Isim,
                                                     Value = i.ID.ToString()
                                                 }).ToList();
            ViewBag.yayineviList = yayineviList;

            List<SelectListItem> dilList = (from i in db.Dil.ToList()
                                            orderby i.Isim
                                            select new SelectListItem
                                            {
                                                Text = i.Isim,
                                                Value = i.ID.ToString()
                                            }).ToList();
            ViewBag.dilList = dilList;

            if (string.IsNullOrEmpty(ISBN)) ISBN = "";
            if (string.IsNullOrEmpty(Adi)) Adi = "";
            if (filterDeweyIds == null) filterDeweyIds = new string[0];
            if (filterYazarIds == null) filterYazarIds = new string[0];
            if (filterYayineviIds == null) filterYayineviIds = new string[0];
            if (filterDilIds == null) filterDilIds = new string[0];

            if (string.IsNullOrEmpty(ISBN) && string.IsNullOrEmpty(Adi) && filterDeweyIds.Length == 0 &&
                filterYazarIds.Length == 0 && filterYayineviIds.Length == 0 && filterDilIds.Length == 0)
            {
                var dataList = db.Kitap.OrderBy(x => x.Adi).ToPagedList(page ?? 1, 10);
                return View(dataList);
            }

            var dataListFilter = db.Kitap.Where(x => x.Adi.Contains(Adi) && x.ISBN.Contains(ISBN));

            if (filterDeweyIds.Length > 0) dataListFilter = dataListFilter.Where(item => filterDeweyIds.Equals(item.DeweyKod));
            if (filterYazarIds.Length > 0) dataListFilter = dataListFilter.Where(item => (item.KitapYazarlari.Where(x => x.KitapId == item.ID).Select(x => x.YazarId).Any(a => filterYazarIds.Any(y => y.ToString() == a.ToString()))));
            if (filterYayineviIds.Length > 0) dataListFilter = dataListFilter.Where(item => filterYayineviIds.Contains(item.YayinEviId.ToString()));
            if (filterDilIds.Length > 0) dataListFilter = dataListFilter.Where(item => filterDilIds.Contains(item.DilId.ToString()));

            var mdataList = dataListFilter.OrderBy(x => x.Adi).ToPagedList(page ?? 1, 10);

            return View(mdataList);
        }

        [HttpGet]
        public ActionResult Insert()
        {
            List<SelectListItem> katList = (from i in db.Kategori.ToList()
                                            orderby i.DeweyId
                                            select new SelectListItem
                                            {
                                                Text = i.DeweyId + " - " + i.Isim,
                                                Value = i.ID.ToString()
                                            }).ToList();
            ViewBag.katList = katList;

            List<SelectListItem> yazarList = (from i in db.Yazar.ToList()
                                              orderby i.Isim, i.Soyisim
                                              select new SelectListItem
                                              {
                                                  Text = i.Isim + " " + i.Soyisim,
                                                  Value = i.ID.ToString()
                                              }).ToList();
            ViewBag.yazarList = yazarList;

            List<SelectListItem> yayineviList = (from i in db.YayinEvi.ToList()
                                                 orderby i.Isim
                                                 select new SelectListItem
                                                 {
                                                     Text = i.Isim,
                                                     Value = i.ID.ToString()
                                                 }).ToList();
            ViewBag.yayineviList = yayineviList;

            List<SelectListItem> dilList = (from i in db.Dil.ToList()
                                            orderby i.Isim
                                            select new SelectListItem
                                            {
                                                Text = i.Isim,
                                                Value = i.ID.ToString()
                                            }).ToList();
            ViewBag.dilList = dilList;

            return View();
        }

        [HttpPost]
        public ActionResult Insert(Kitap kitap, int[] yazarIds)
        {
            if (isInsert)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.Adi))
            {
                ModelState.AddModelError("Adi", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.ISBN))
            {
                ModelState.AddModelError("ISBN", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.Cevirmen))
            {
                ModelState.AddModelError("Cevirmen", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.Sayfa.ToString()))
            {
                ModelState.AddModelError("Sayfa", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.IlkBasimYili.ToString()))
            {
                ModelState.AddModelError("IlkBasimYili", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.Ebat))
            {
                ModelState.AddModelError("Ebat", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.BasimSayisi.ToString()))
            {
                ModelState.AddModelError("BasimSayisi", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (yazarIds == null)
            {
                ModelState.AddModelError("yazarDropDown", "");
                return Json(data: new { success = 1, message = "YAZAR BİLGİSİNİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                isInsert = true;

                try
                {
                    int data = db.Kitap.Where(i => i.Adi == kitap.Adi).Count();
                    if (data != 0)
                    {
                        isInsert = false;
                        return Json(data: new { success = 3, message = "KİTAP ADI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                    }

                    data = db.Kitap.Where(i => i.ISBN == kitap.ISBN).Count();
                    if (data != 0)
                    {
                        isInsert = false;
                        return Json(data: new { success = 3, message = "ISBN NUMARASI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                    }

                    kitap.EmaneteUygunmu = true;
                    db.Kitap.Add(kitap);

                    for (int i = 0; i < yazarIds.Length; i++)
                    {
                        KitapYazarlari kitapYazarlari = new KitapYazarlari() { KitapId = kitap.ID, YazarId = yazarIds[i] };
                        db.KitapYazarlari.Add(kitapYazarlari);
                    }

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

        [HttpGet]
        public ActionResult Update(int ID)
        {
            List<SelectListItem> katList = (from i in db.Kategori.ToList()
                                            orderby i.DeweyId
                                            select new SelectListItem
                                            {
                                                Text = i.DeweyId + " - " + i.Isim,
                                                Value = i.ID.ToString()
                                            }).ToList();
            ViewBag.katList = katList;

            List<SelectListItem> yazarList = (from i in db.Yazar
                                              orderby i.Isim, i.Soyisim
                                              select new SelectListItem
                                              {
                                                  Text = i.Isim + " " + i.Soyisim,
                                                  Value = i.ID.ToString(),
                                                  Selected = db.KitapYazarlari.Any(x => x.YazarId == i.ID && x.KitapId == ID)
                                              }).ToList();
            ViewBag.yazarList = yazarList;

            List<SelectListItem> yayineviList = (from i in db.YayinEvi.ToList()
                                                 orderby i.Isim
                                                 select new SelectListItem
                                                 {
                                                     Text = i.Isim,
                                                     Value = i.ID.ToString()
                                                 }).ToList();
            ViewBag.yayineviList = yayineviList;

            List<SelectListItem> dilList = (from i in db.Dil.ToList()
                                            orderby i.Isim
                                            select new SelectListItem
                                            {
                                                Text = i.Isim,
                                                Value = i.ID.ToString()
                                            }).ToList();
            ViewBag.dilList = dilList;


            var data = db.Kitap.Where(item => item.ID == ID).Single();
            if (data != null)
            {
                return View("Update", data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Update(Kitap kitap, int[] yazarIds)
        {
            if (isUpdate)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.Adi))
            {
                ModelState.AddModelError("Adi", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.ISBN))
            {
                ModelState.AddModelError("ISBN", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.Cevirmen))
            {
                ModelState.AddModelError("Cevirmen", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.Sayfa.ToString()))
            {
                ModelState.AddModelError("Sayfa", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.IlkBasimYili.ToString()))
            {
                ModelState.AddModelError("IlkBasimYili", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.Ebat))
            {
                ModelState.AddModelError("Ebat", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(kitap.BasimSayisi.ToString()))
            {
                ModelState.AddModelError("BasimSayisi", "");
                return Json(data: new { success = 1, message = "BOŞ YERLERİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (yazarIds == null)
            {
                ModelState.AddModelError("yazarDropDown", "");
                return Json(data: new { success = 1, message = "YAZAR BİLGİSİNİ DOLDURUNUZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                isUpdate = true;

                try
                {
                    int data = db.Kitap.Where(i => i.Adi == kitap.Adi && i.ID != kitap.ID).Count();
                    if (data != 0)
                    {
                        isInsert = false;
                        return Json(data: new { success = 3, message = "KİTAP ADI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                    }

                    data = db.Kitap.Where(i => i.ISBN == kitap.ISBN && i.ID != kitap.ID).Count();
                    if (data != 0)
                    {
                        isInsert = false;
                        return Json(data: new { success = 3, message = "ISBN NUMARASI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                    }


                    // Burada Kayıt İşlemi
                    var localData = db.Kitap.Where(item => item.ID == kitap.ID).SingleOrDefault();
                    localData.Adi = kitap.Adi;
                    localData.ISBN = kitap.ISBN;
                    localData.DeweyKod = kitap.DeweyKod;
                    localData.YayinEviId = kitap.YayinEviId;
                    localData.Cevirmen = kitap.Cevirmen;
                    localData.Sayfa = kitap.Sayfa;
                    localData.IlkBasimYili = kitap.IlkBasimYili;
                    localData.Ebat = kitap.Ebat;
                    localData.BasimSayisi = kitap.BasimSayisi;
                    localData.DilId = kitap.DilId;
                    localData.CiltTipi = kitap.CiltTipi;
                    localData.Aciklama = kitap.Aciklama;


                    if (yazarIds != null)
                    {
                        var yazarDatas = db.KitapYazarlari.Where(x => x.KitapId == kitap.ID);
                        db.KitapYazarlari.RemoveRange(yazarDatas);
                        db.SaveChanges();

                        for (int i = 0; i < yazarIds.Length; i++)
                        {
                            KitapYazarlari kitapYazarlari = new KitapYazarlari() { KitapId = kitap.ID, YazarId = yazarIds[i] };
                            db.KitapYazarlari.Add(kitapYazarlari);
                        }
                    }

                    db.SaveChanges();

                    isUpdate = false;
                    return Json(data: new { success = 0, message = "BİLGİLERİ GÜNCELLENDİ!" }, JsonRequestBehavior.AllowGet);

                }
                // Herhangi bir hata durumunda kayıt işleminin devam ettiğini belirten değişkenimizi false yapıyoruz
                catch (Exception e) { isUpdate = false; }
            }
            else
            {
                // Validate hatası olursa buraya giriyor
                return Json(data: new { success = 3, message = "LÜTFEN HATALARI DÜZELTİNİZ!" }, JsonRequestBehavior.AllowGet);
            }

            // Bilmediğim bir hata olursa bitiriyor
            return Json(data: new { success = 1, message = "BİR HATA OLDU!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int kitapID)
        {
            if (isDelete)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            isDelete = true;
            var datas = db.Kitap.Where(x => x.ID == kitapID).SingleOrDefault();
            if (datas != null)
            {
                // Kitap Silme Olmayacak - Devre dışı bırak olacak
                // Emanet Verilmişse olmayacak

                db.Kitap.Remove(datas);

                var yazarDatas = db.KitapYazarlari.Where(x => x.KitapId == kitapID);
                db.KitapYazarlari.RemoveRange(yazarDatas);

                db.SaveChanges();

                isDelete = false;
                return Json(data: new { success = 0, message = "SİLME İŞLEMİ BAŞARILI!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(data: new { success = -1, message = "" }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Detail(int ID)
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
                return View("Detail", data);
            }

            return View();
        }
    }
}