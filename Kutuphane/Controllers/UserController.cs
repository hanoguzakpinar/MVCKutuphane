using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Kutuphane.Models;
using Kutuphane.Models.Entity;

namespace Kutuphane.Controllers
{
    public class UserController : Controller
    {
        dbLibrarySomeeEntities1 db = new dbLibrarySomeeEntities1();
        private bool isInsert = false;
        private bool isUpdate = false;
        private bool isDelete = false;

        // GET: User
        [Authorize]
        public ActionResult Index(string Tc, string Adi, string Soyadi, string MinDogumTarihi, string MaxDogumTarihi,
            string MinUyelikTarihi, string MaxUyelikTarihi, string Cinsiyet, string Yetki, int? page)
        {
            if (string.IsNullOrEmpty(Tc)) Tc = "";
            if (string.IsNullOrEmpty(Adi)) Adi = "";
            if (string.IsNullOrEmpty(Soyadi)) Soyadi = "";
            if (string.IsNullOrEmpty(MinDogumTarihi)) MinDogumTarihi = "";
            if (string.IsNullOrEmpty(MaxDogumTarihi)) MaxDogumTarihi = "";
            if (string.IsNullOrEmpty(MinUyelikTarihi)) MinUyelikTarihi = "";
            if (string.IsNullOrEmpty(MaxUyelikTarihi)) MaxUyelikTarihi = "";
            if (Cinsiyet == "-1") Cinsiyet = "";
            if (Yetki == "-1") Yetki = "";


            if (string.IsNullOrEmpty(Tc) && string.IsNullOrEmpty(Adi) && string.IsNullOrEmpty(Soyadi) && string.IsNullOrEmpty(MinDogumTarihi) &&
                string.IsNullOrEmpty(MaxDogumTarihi) && string.IsNullOrEmpty(MinUyelikTarihi) && string.IsNullOrEmpty(MaxUyelikTarihi) &&
                string.IsNullOrEmpty(Cinsiyet) && string.IsNullOrEmpty(Yetki))
            {
                var dataList = db.Users.OrderBy(x => x.Adi).ToPagedList(page ?? 1, 10);
                return View(dataList);
            }


            Cinsiyet = Cinsiyet == "0" ? "Erkek" : Cinsiyet == "1" ? "Kadın" : "";


            var dataListFilter = db.Users.Where(x => x.Tc.Contains(Tc) && x.Adi.Contains(Adi) && x.Soyadi.Contains(Soyadi) &&
            x.Cinsiyet.Contains(Cinsiyet));

            if (!string.IsNullOrEmpty(MinDogumTarihi))
            {
                DateTime dateMin = Convert.ToDateTime(MinDogumTarihi);
                dataListFilter = dataListFilter.Where(x => x.DogumTarihi >= dateMin);
            }
            if (!string.IsNullOrEmpty(MaxDogumTarihi))
            {
                DateTime dateMax = Convert.ToDateTime(MaxDogumTarihi);
                dataListFilter = dataListFilter.Where(x => x.DogumTarihi <= dateMax);
            }
            if (!string.IsNullOrEmpty(MinUyelikTarihi))
            {
                DateTime dateMin = Convert.ToDateTime(MinUyelikTarihi);
                dataListFilter = dataListFilter.Where(x => x.UyelikTarihi >= dateMin);
            }
            if (!string.IsNullOrEmpty(MaxUyelikTarihi))
            {
                DateTime dateMax = Convert.ToDateTime(MaxUyelikTarihi);
                dataListFilter = dataListFilter.Where(x => x.UyelikTarihi <= dateMax);
            }
            if (!string.IsNullOrEmpty(Yetki))
            {
                int _yetki = int.Parse(Yetki);
                dataListFilter = dataListFilter.Where(x => x.Yetki == _yetki);
            }

            var mdataList = dataListFilter.OrderBy(x => x.Adi).ThenBy(x => x.Soyadi).ThenBy(x => x.Tc).ToPagedList(page ?? 1, 5);

            return View(mdataList);
        }

        [HttpGet]
        public ActionResult Insert()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insert(Users user)
        {
            if (isInsert)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            #region boş bırakılamaz ifler
            /*if (string.IsNullOrEmpty(user.Tc))
            {
                ModelState.AddModelError("Tc", "");
                return Json(data: new { success = 1, message = "TC NO BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(user.Adi))
            {
                ModelState.AddModelError("Adi", "");
                return Json(data: new { success = 1, message = "İSİM BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(user.Soyadi))
            {
                ModelState.AddModelError("Soyadi", "");
                return Json(data: new { success = 1, message = "SOYİSİM BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (user.DogumTarihi == null)
            {
                ModelState.AddModelError("DogumTarihi", "");
                return Json(data: new { success = 1, message = "DOĞUM TARİHİ BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(user.DogumYeri))
            {
                ModelState.AddModelError("DogumYeri", "");
                return Json(data: new { success = 1, message = "DOĞRUM YERİ BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (user.UyelikTarihi != null)
            {
                ModelState.AddModelError("UyelikTarihi", "");
                return Json(data: new { success = 1, message = "ÜYELİK TARİHİ BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(user.Cinsiyet))
            {
                ModelState.AddModelError("Cinsiyet", "");
                return Json(data: new { success = 1, message = "CİNSİYET BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(user.Adres))
            {
                ModelState.AddModelError("Adres", "");
                return Json(data: new { success = 1, message = "ADRES BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("Email", "");
                return Json(data: new { success = 1, message = "EMAİL BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(user.Username))
            {
                ModelState.AddModelError("Username", "");
                return Json(data: new { success = 1, message = "KULLANICI ADI BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                ModelState.AddModelError("Password", "");
                return Json(data: new { success = 1, message = "ŞİFRE BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }

            if (user.Yetki == -1)
            {
                ModelState.AddModelError("Yetki", "");
                return Json(data: new { success = 1, message = "YETKİ BOŞ BIRAKILAMAZ!" }, JsonRequestBehavior.AllowGet);
            }*/
            #endregion

            isInsert = true;

            try
            {
                int data = db.Users.Where(i => i.Tc == user.Tc).Count();
                if (data != 0)
                {
                    isInsert = false;
                    return Json(data: new { success = 3, message = "TC NO SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                }

                data = db.Users.Where(i => i.Telefon == user.Telefon).Count();
                if (data != 0)
                {
                    isInsert = false;
                    return Json(data: new { success = 3, message = "TELEFON NUMARASI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                }

                data = db.Users.Where(i => i.Email == user.Email).Count();
                if (data != 0)
                {
                    isInsert = false;
                    return Json(data: new { success = 3, message = "EMAİL SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                }

                data = db.Users.Where(i => i.Username == user.Username).Count();
                if (data != 0)
                {
                    isInsert = false;
                    return Json(data: new { success = 3, message = "KULLANICI ADI NUMARASI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                }

                user.Cinsiyet = user.Cinsiyet == "0" ? "Erkek" : user.Cinsiyet == "1" ? "Kadın" : "";
                user.UyelikTarihi = DateTime.Now;

                db.Users.Add(user);
                db.SaveChanges();

                isInsert = false;
                return Json(data: new { success = 0, message = "KAYIT BAŞARILI!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e) { isInsert = false; }

            return Json(data: new { success = 1, message = "BİR HATA OLDU!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Update(int ID)
        {
            var data = db.Users.Where(item => item.ID == ID).Single();
            if (data != null)
            {
                int cins = data.Cinsiyet == "Erkek" ? 0 : 1;
                ViewBag.cinsiyet = cins;

                return View("Update", data);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Update(Users user)
        {
            if (isUpdate)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            isUpdate = true;

            try
            {
                int data = db.Users.Where(i => i.Tc == user.Tc && i.ID != user.ID).Count();
                if (data != 0)
                {
                    isInsert = false;
                    return Json(data: new { success = 3, message = "TC NO SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                }

                data = db.Users.Where(i => i.Telefon == user.Telefon && i.ID != user.ID).Count();
                if (data != 0)
                {
                    isInsert = false;
                    return Json(data: new { success = 3, message = "TELEFON NUMARASI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                }

                data = db.Users.Where(i => i.Email == user.Email && i.ID != user.ID).Count();
                if (data != 0)
                {
                    isInsert = false;
                    return Json(data: new { success = 3, message = "EMAİL SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                }

                data = db.Users.Where(i => i.Username == user.Username && i.ID != user.ID).Count();
                if (data != 0)
                {
                    isInsert = false;
                    return Json(data: new { success = 3, message = "KULLANICI ADI SİSTEMDE MEVCUT!" }, JsonRequestBehavior.AllowGet);
                }

                user.Cinsiyet = user.Cinsiyet == "0" ? "Erkek" : user.Cinsiyet == "1" ? "Kadın" : "";

                // Burada Kayıt İşlemi
                var localData = db.Users.Where(item => item.ID == user.ID).SingleOrDefault();
                localData.Tc = user.Tc;
                localData.Adi = user.Adi;
                localData.Soyadi = user.Soyadi;
                localData.DogumTarihi = user.DogumTarihi;
                localData.DogumYeri = user.DogumYeri;
                localData.Telefon = user.Telefon;
                localData.Cinsiyet = user.Cinsiyet;
                localData.Adres = user.Adres;
                localData.Email = user.Email;
                localData.Username = user.Username;
                localData.Password = user.Password;
                localData.Yetki = user.Yetki;

                db.SaveChanges();

                isUpdate = false;
                return Json(data: new { success = 0, message = "BİLGİLERİ GÜNCELLENDİ!" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e) { isUpdate = false; }

            return Json(data: new { success = 1, message = "BİR HATA OLDU!" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int userId)
        {
            if (isDelete)
            {
                return Json(data: new { success = 2, message = "İŞLEM DEVAM EDİYOR!" }, JsonRequestBehavior.AllowGet);
            }

            isDelete = true;
            var datas = db.Users.Where(x => x.ID == userId).SingleOrDefault();
            if (datas != null)
            {
                db.Users.Remove(datas);
                db.SaveChanges();

                isDelete = false;
                return Json(data: new { success = 0, message = "SİLME İŞLEMİ BAŞARILI!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(data: new { success = -1, message = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Detail(int ID)
        {
            var data = db.Users.Where(item => item.ID == ID).Single();
            if (data != null)
            {
                string yetki = data.Yetki.ToString();
                yetki = yetki == "0" ? "Kullanıcı" : yetki == "1" ? "Personel" : yetki == "2" ? "Yönetici" : "Belirtilmemiş";
                ViewBag.yetki = yetki;


                return View("Detail", data);
            }

            return View();
        }
    }
}