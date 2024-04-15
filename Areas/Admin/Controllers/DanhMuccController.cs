using DoAnCoSoNghanh.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class DanhMuccController : Controller
    {
        // GET: Admin/DanhMuc
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

                // Tiếp tục thực hiện các thao tác cần thiết khi người dùng đã đăng nhập và có quyền admin.
                var listLoaiViecLam = new List<LoaiVieclam>();
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
                    listLoaiViecLam = db.LoaiVieclams.Where(n => n.TenLoai.Contains(SearchString)).ToList();
                }
                else
                {
                    listLoaiViecLam = db.LoaiVieclams.ToList();

                }
                ViewBag.CurrentFilter = SearchString;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                listLoaiViecLam = listLoaiViecLam.OrderByDescending(n => n.Mal).ToList();
                return View(listLoaiViecLam.ToPagedList(pageNumber, pageSize));
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
        public ActionResult Them(LoaiVieclam loaiVieclam)
        {
            db.LoaiVieclams.InsertOnSubmit(loaiVieclam);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Chitiet(int? id)
        {
            var chitietdanhmuc = db.LoaiVieclams.Where(n => n.Mal == id).FirstOrDefault();
            return View(chitietdanhmuc);
        }


        [HttpGet]
        public ActionResult Sua(int? id)
        {
            var sua = db.LoaiVieclams.Where(n => n.Mal == id).FirstOrDefault();
            return View(sua);
        }

        [HttpPost]
        public ActionResult Sua(LoaiVieclam loaiVieclam)
        {
            LoaiVieclam uvl = db.LoaiVieclams.FirstOrDefault(x => x.Mal == loaiVieclam.Mal);
            uvl.TenLoai = loaiVieclam.TenLoai;
            uvl.Icon = loaiVieclam.Icon;
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Xoa(int id)
        {
            var lvl = db.LoaiVieclams.FirstOrDefault(n => n.Mal == id);

            var relatedViecLams = db.ViecLams.Where(v => v.Mal == lvl.Mal).ToList();

            // Delete related records in ViecLam
            foreach (var viecLam in relatedViecLams)
            {
                // Optionally, delete related records in DonUngTuyen or other tables if needed
                foreach (var donUngTuyen in viecLam.DonUngTuyens.ToList())
                {
                    // Delete related records in LuuViec
                    var relatedLuuViecs = db.LuuViecs.Where(l => l.MaCongViec == viecLam.MaCongViec).ToList();
                    foreach (var luuViec in relatedLuuViecs)
                    {
                        db.LuuViecs.DeleteOnSubmit(luuViec);
                    }

                    db.DonUngTuyens.DeleteOnSubmit(donUngTuyen);
                }

                db.ViecLams.DeleteOnSubmit(viecLam);
            }

            db.LoaiVieclams.DeleteOnSubmit(lvl);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }




    }
}




