using Kutuphane.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace Kutuphane.Controllers
{
    public class EmanetVerController : Controller
    {
        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();
        private bool isInsert = false;
        private bool isUpdate = false;
        private bool isDelete = false;

        // GET: EmanetVer
        [Authorize]
        public ActionResult Index(string[] filterBookIds, string[] filterUserIds, string MinAldigiTarih, string MaxAldigiTarih, 
            string MinVerecegiTarih, string MaxVerecegiTarih, string MinVerdigiTarih, string MaxVerdigiTarih, string MinBorc,
            string MaxBorc, string TeslimDurumu, int? page)
        {
            if (filterBookIds == null) filterBookIds = new string[0];
            if (filterUserIds == null) filterUserIds = new string[0];
            if (string.IsNullOrEmpty(MinAldigiTarih)) MinAldigiTarih = "";
            if (string.IsNullOrEmpty(MaxAldigiTarih)) MaxAldigiTarih = "";
            if (string.IsNullOrEmpty(MinVerecegiTarih)) MinVerecegiTarih = "";
            if (string.IsNullOrEmpty(MaxVerecegiTarih)) MaxVerecegiTarih = "";
            if (string.IsNullOrEmpty(MinVerdigiTarih)) MinVerdigiTarih = "";
            if (string.IsNullOrEmpty(MaxVerdigiTarih)) MaxVerdigiTarih = "";
            if (string.IsNullOrEmpty(MinBorc)) MinBorc = "";
            if (string.IsNullOrEmpty(MaxBorc)) MaxBorc = "";
            if (string.IsNullOrEmpty(TeslimDurumu)) TeslimDurumu = "";
            if (TeslimDurumu == "-1") TeslimDurumu = "";


            viewbagIndex();


            if (filterBookIds.Length == 0 && filterUserIds.Length == 0 && string.IsNullOrEmpty(MinAldigiTarih) && 
                string.IsNullOrEmpty(MaxAldigiTarih) && string.IsNullOrEmpty(MinVerecegiTarih) && string.IsNullOrEmpty(MaxVerecegiTarih) && 
                string.IsNullOrEmpty(MinVerdigiTarih) && string.IsNullOrEmpty(MaxVerdigiTarih) && string.IsNullOrEmpty(MinBorc) &&
                string.IsNullOrEmpty(MaxBorc) && string.IsNullOrEmpty(TeslimDurumu))
            {
                var dataList = db.ViewEmanetKitaplar.OrderByDescending(x => x.AldigiTarih).ThenBy(x => x.TeslimDurumu).ToPagedList(page ?? 1, 10);
                return View(dataList);
            }



            var dataListFilter = db.ViewEmanetKitaplar.ToList().Where(x => x.TeslimDurumu == true || x.TeslimDurumu == false);

            if (filterBookIds.Length > 0) 
                dataListFilter = dataListFilter.Where(item => filterBookIds.Contains(item.KitapId.ToString()));
            if (filterUserIds.Length > 0) 
                dataListFilter = dataListFilter.Where(item => filterUserIds.Contains(item.KullaniciId.ToString()));

            if (!string.IsNullOrEmpty(MinAldigiTarih))
            {
                DateTime dateMin = Convert.ToDateTime(MinAldigiTarih);
                dataListFilter = dataListFilter.Where(x => x.AldigiTarih >= dateMin);
            }
            if (!string.IsNullOrEmpty(MaxAldigiTarih))
            {
                DateTime dateMax = Convert.ToDateTime(MaxAldigiTarih);
                dataListFilter = dataListFilter.Where(x => x.AldigiTarih <= dateMax);
            }

            if (!string.IsNullOrEmpty(MinVerecegiTarih))
            {
                DateTime dateMin = Convert.ToDateTime(MinVerecegiTarih);
                dataListFilter = dataListFilter.Where(x => x.VerecegiTarih >= dateMin);
            }
            if (!string.IsNullOrEmpty(MaxVerecegiTarih))
            {
                DateTime dateMax = Convert.ToDateTime(MaxVerecegiTarih);
                dataListFilter = dataListFilter.Where(x => x.VerecegiTarih <= dateMax);
            }

            if (!string.IsNullOrEmpty(MinVerdigiTarih))
            {
                DateTime dateMin = Convert.ToDateTime(MinVerdigiTarih);
                dataListFilter = dataListFilter.Where(x => x.VerdigiTarih >= dateMin);
            }
            if (!string.IsNullOrEmpty(MaxVerdigiTarih))
            {
                DateTime dateMax = Convert.ToDateTime(MaxVerdigiTarih);
                dataListFilter = dataListFilter.Where(x => x.VerdigiTarih <= dateMax);
            }

            if (!string.IsNullOrEmpty(MinBorc))
            {
                decimal _min = Convert.ToDecimal(MinBorc);
                dataListFilter = dataListFilter.Where(x => x.Borcu >= _min);
            }
            if (!string.IsNullOrEmpty(MaxBorc))
            {
                decimal _max = Convert.ToDecimal(MaxBorc);
                dataListFilter = dataListFilter.Where(x => x.Borcu <= _max);
            }

            if (!string.IsNullOrEmpty(TeslimDurumu))
            {
                bool isTeslim = TeslimDurumu == "0" ? false : true;
                dataListFilter = dataListFilter.Where(x => x.TeslimDurumu == isTeslim);
            }


            var mdataListFilter = dataListFilter.ToPagedList(page ?? 1, 10);
            return View(mdataListFilter);
        }

        private void viewbagIndex()
        {
            List<SelectListItem> bookList = (from i in db.Kitap
                                             orderby i.ISBN, i.Adi
                                             select new SelectListItem
                                             {
                                                 Text = i.ISBN + " - " + i.Adi,
                                                 Value = i.ID.ToString()
                                             }).ToList();
            ViewBag.bookList = bookList;

            List<SelectListItem> userList = (from i in db.Users
                                             orderby i.Adi, i.Soyadi
                                             select new SelectListItem
                                             {
                                                 Text = i.Adi + " " + i.Soyadi,
                                                 Value = i.ID.ToString()
                                             }).ToList();
            ViewBag.userList = userList;
        }

        [HttpGet]
        public ActionResult Insert()
        {
            List<SelectListItem> kitapList = (from i in db.Kitap.ToList()
                                              where i.EmaneteUygunmu == true
                                              orderby i.ISBN, i.Adi
                                              select new SelectListItem
                                              {
                                                  Text = i.ISBN + " - " + i.Adi,
                                                  Value = i.ID.ToString()
                                              }).ToList();
            ViewBag.kitapList = kitapList;

            List<SelectListItem> userList = (from i in db.Users.ToList()
                                              where i.Yetki == (int)Users.Yetkiler.Kullanıcı
                                              orderby i.Adi, i.Soyadi
                                              select new SelectListItem
                                              {
                                                  Text = i.Adi + " " + i.Soyadi,
                                                  Value = i.ID.ToString()
                                              }).ToList();
            ViewBag.userList = userList;

            string dateNow = DateTime.Now.ToString("MM/dd/yyyy");
            string dateNext = DateTime.Now.AddDays(30).ToString("MM/dd/yyyy");

            ViewBag.dateNow = dateNow;
            ViewBag.dateNext = dateNext;

            return View();
        }

        [HttpPost]
        public ActionResult Insert(EmanetKitaplar emanet)
        {
            if (isInsert)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            isInsert = true;

            try
            {
                emanet.AldigiTarih = DateTime.Now;
                emanet.VerecegiTarih = DateTime.Now.AddDays(30);
                emanet.Borcu = 0;
                emanet.TeslimDurumu = false;

                var kitap = db.Kitap.Where(x => x.ID == emanet.KitapId).SingleOrDefault();
                kitap.EmaneteUygunmu = false;

                db.EmanetKitaplar.Add(emanet);
                db.SaveChanges();

                SelectListItem kitapList = (from i in db.Kitap.ToList()
                                                  where i.EmaneteUygunmu == true
                                                  orderby i.ISBN, i.Adi
                                                  select new SelectListItem
                                                  {
                                                      Text = i.ISBN + " - " + i.Adi,
                                                      Value = i.ID.ToString()
                                                  }).Take(1).FirstOrDefault();

                isInsert = false;
                if (kitapList != null)
                {
                    return Json(data: new { success = 0, firstID = kitapList.Value, message = "KAYIT BAŞARILI!" }, JsonRequestBehavior.AllowGet);
                }
                return Json(data: new { success = 0, message = "KAYIT BAŞARILI!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e) { isInsert = false; }

            return Json(data: new { success = 1, message = "BİR HATA OLDU!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Update(int ID)
        {
            var data = db.ViewEmanetKitaplar.Where(item => item.ID == ID).Single();
            if (data != null)
            {
                List<SelectListItem> kitapList = (from i in db.Kitap.ToList()
                                                  where i.EmaneteUygunmu == true || i.ID == data.KitapId
                                                  orderby i.ISBN, i.Adi
                                                  select new SelectListItem
                                                  {
                                                      Text = i.ISBN + " - " + i.Adi,
                                                      Value = i.ID.ToString()
                                                  }).ToList();
                ViewBag.kitapList = kitapList;

                List<SelectListItem> userList = (from i in db.Users.ToList()
                                                 where i.Yetki == (int)Users.Yetkiler.Kullanıcı
                                                 orderby i.Adi, i.Soyadi
                                                 select new SelectListItem
                                                 {
                                                     Text = i.Adi + " " + i.Soyadi,
                                                     Value = i.ID.ToString()
                                                 }).ToList();
                ViewBag.userList = userList;

                return View("Update", data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Update(EmanetKitaplar emanetKitaplar)
        {
            if (isUpdate)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            isUpdate = true;

            try
            {
                var localData = db.EmanetKitaplar.Where(item => item.ID == emanetKitaplar.ID).Single();
                if (localData.TeslimDurumu != true)
                {
                    localData.VerdigiTarih = emanetKitaplar.VerdigiTarih;
                    localData.Borcu = emanetKitaplar.Borcu;
                    localData.HasarDurumu = emanetKitaplar.HasarDurumu;
                    localData.TeslimDurumu = true;
                    localData.HasarYeri = emanetKitaplar.HasarYeri;

                    var localKitap = db.Kitap.Where(x => x.ID == emanetKitaplar.KitapId).Single();
                    localKitap.EmaneteUygunmu = true;

                    db.SaveChanges();

                    isUpdate = false;
                    return Json(data: new { success = 0, message = "BİLGİLERİ GÜNCELLENDİ!" }, JsonRequestBehavior.AllowGet);
                }

                isUpdate = false;
                return Json(data: new { success = 3, message = "EMANET TESLİM ALINMIŞ!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e) { isUpdate = false; }

            return Json(data: new { success = 1, message = "BİR HATA OLDU!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(int ID)
        {
            var data = db.ViewEmanetKitaplar.Where(item => item.ID == ID).Single();

            ViewBag.teslim = "Teslim Edilmedi";
            if (data.TeslimDurumu == true)
            {
                ViewBag.teslim = "Teslim Edildi";
            }


            if (data != null)
            {
                return View("Detail", data);
            }

            return View();
        }
    }
}