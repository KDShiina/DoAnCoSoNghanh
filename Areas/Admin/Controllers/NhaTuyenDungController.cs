using DoAnCoSoNghanh.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class NhaTuyenDungController : Controller
    {
        // GET: Admin/NhaTuyenDung
        TimViecDataContext db = new TimViecDataContext();
        public ActionResult Index(string currentFilter, string SearchString, int? page)
        {
            if (Session["KhachHang"] != null)
            {
                // Nếu đã đăng nhập, kiểm tra quyền và thực hiện các thao tác cần thiết
                KhachHang khachHang = (KhachHang)Session["KhachHang"];

                // Kiểm tra quyền của người dùng. Nếu quyền không phải là admin, chuyển hướng về trang đăng nhập.
                if (khachHang.IDQuyen != 1)
                {
                    return RedirectToAction("DangNhap", "Home", new { area = "" });
                }

                var listNhatuyendung = new List<NhaTuyenDung>();
                if (SearchString != null)
                {
                    page = 1;
                }
                else
                {
                    SearchString = currentFilter;
                }
                if (!string.IsNullOrEmpty(SearchString))
                {
                    listNhatuyendung = db.NhaTuyenDungs.Where(n => n.TenCongTy.Contains(SearchString)).ToList();
                }
                else
                {
                    listNhatuyendung = db.NhaTuyenDungs.ToList();

                }
                ViewBag.CurrentFilter = SearchString;
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                listNhatuyendung = listNhatuyendung.OrderByDescending(n => n.MaNTD).ToList();

                return View(listNhatuyendung.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập.
                return RedirectToAction("DangNhap", "Home", new { area = "" });
            }
        }

            [HttpGet]
        public ActionResult Them()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Them(NhaTuyenDung nhaTuyenDung)
        {
            try
            {
                if(nhaTuyenDung.ImageUpload !=null)
                {
                    string fileName=Path.GetFileNameWithoutExtension(nhaTuyenDung.ImageUpload.FileName);
                    string extension = Path.GetExtension(nhaTuyenDung.ImageUpload.FileName);
                    fileName = fileName  +"_"+ long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss"))+ extension;
                    nhaTuyenDung.Hinh = fileName;
                    nhaTuyenDung.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"),fileName));
                }
                db.NhaTuyenDungs.InsertOnSubmit(nhaTuyenDung);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
               
                return RedirectToAction("Index");
            }
           
        }

        [HttpGet]
        public ActionResult Chitiet(int? id)
        {
            var chitietnhatuyendung = db.NhaTuyenDungs.Where(n => n.MaNTD == id).FirstOrDefault();
            return View(chitietnhatuyendung);
        }

        [HttpGet]
        public ActionResult Sua(int? id)
        {
            var sua = db.NhaTuyenDungs.Where(n => n.MaNTD == id).FirstOrDefault();
            return View(sua);
        }

        [HttpPost]
        public ActionResult Sua(NhaTuyenDung nhaTuyenDung)
        {
            NhaTuyenDung ukh = db.NhaTuyenDungs.FirstOrDefault(x => x.MaNTD == nhaTuyenDung.MaNTD);
            if (nhaTuyenDung.ImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(nhaTuyenDung.ImageUpload.FileName);
                string extension = Path.GetExtension(nhaTuyenDung.ImageUpload.FileName);
                fileName = fileName + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                nhaTuyenDung.Hinh = fileName;
                nhaTuyenDung.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
            }
            else
            {
                nhaTuyenDung.Hinh = ukh.Hinh;
            }
            ukh.TenCongTy = nhaTuyenDung.TenCongTy;
            ukh.MoTaCongTy = nhaTuyenDung.MoTaCongTy;
            ukh.Hinh = nhaTuyenDung.Hinh;
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Xoa(int id)
        {
            var viecLamRecords = db.ViecLams.Where(v => v.MaNTD == id);
            foreach (var viecLam in viecLamRecords)
            {
                // Remove any reference to this Nhà tuyển dụng in job listings
                viecLam.MaNTD = null;
                db.SubmitChanges();

                // Now, you can delete the Nhà tuyển dụng
                db.ViecLams.DeleteOnSubmit(viecLam);
            }
           

            var luuViecRecords = db.LuuViecs.Where(lv => lv.MaNTD == id);
            db.LuuViecs.DeleteAllOnSubmit(luuViecRecords);

          
            // No job listings, proceed with deletion
            var donUngTuyenRecords = db.DonUngTuyens.Where(d => d.MaNTD == id);

            foreach (var donUngTuyen in donUngTuyenRecords)
            {
                db.DonUngTuyens.DeleteOnSubmit(donUngTuyen);
            }
           
            var ntd = db.NhaTuyenDungs.FirstOrDefault(n => n.MaNTD == id);

            db.NhaTuyenDungs.DeleteOnSubmit(ntd);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }






    }
}