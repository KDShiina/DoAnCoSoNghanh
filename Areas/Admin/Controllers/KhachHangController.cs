using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DoAnCoSoNghanh.Models;
using PagedList;
using PagedList.Mvc;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: Admin/KhachHang
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

                var listKhachHang = new List<KhachHang>();
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
                    listKhachHang = db.KhachHangs
                  .Where(n => n.HoTen.Contains(SearchString) || n.MaKH.ToString().Contains(SearchString))
                  .ToList();
                }
                else
                {
                    listKhachHang = db.KhachHangs.ToList();

                }
                ViewBag.CurrentFilter = SearchString;
                int pageSize = 4;
                int pageNumber = (page ?? 1);
                listKhachHang = listKhachHang.OrderByDescending(n => n.MaKH).ToList();
                return View(listKhachHang.ToPagedList(pageNumber, pageSize));
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
        public ActionResult Them(KhachHang khachhang)
        {
            db.KhachHangs.InsertOnSubmit(khachhang);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Chitiet(int ? id)
        {
            var chitietkhachhang=db.KhachHangs.Where(n=>n.MaKH == id).FirstOrDefault();
            return View(chitietkhachhang);
        }


        [HttpGet]
        public ActionResult Xoa(int id)
        {
            var khachHangToDelete = db.KhachHangs.FirstOrDefault(n => n.MaKH == id);

            if (khachHangToDelete != null)
            {
                // Delete related records in NhaTuyenDung
                var nhaTuyenDungList = db.NhaTuyenDungs.Where(n => n.MaKH == id).ToList();
                foreach (var ntd in nhaTuyenDungList)
                {
                    // Delete related records in DonUngTuyen
                    var donUngTuyenList = db.DonUngTuyens.Where(d => d.MaNTD == ntd.MaNTD).ToList();
                    db.DonUngTuyens.DeleteAllOnSubmit(donUngTuyenList);
                }

                // Now delete the related records in NhaTuyenDung
                db.NhaTuyenDungs.DeleteAllOnSubmit(nhaTuyenDungList);

                // Delete related records in ViecLam
                var viecLamList = db.ViecLams.Where(v => v.MaKH == id).ToList();
                foreach (var vl in viecLamList)
                {
                    // Delete related records in DonUngTuyen
                    var donUngTuyenList = db.DonUngTuyens.Where(d => d.MaCongViec == vl.MaCongViec).ToList();
                    db.DonUngTuyens.DeleteAllOnSubmit(donUngTuyenList);

                    // Delete related records in LuuViec
                    var luuViecList = db.LuuViecs.Where(lv => lv.MaCongViec == vl.MaCongViec).ToList();
                    db.LuuViecs.DeleteAllOnSubmit(luuViecList);
                }

                // Now delete the related records in ViecLam
                db.ViecLams.DeleteAllOnSubmit(viecLamList);

                // Delete related records in DonUngTuyen
                var donUngTuyenListKhachHang = db.DonUngTuyens.Where(d => d.MaKH == id).ToList();
                db.DonUngTuyens.DeleteAllOnSubmit(donUngTuyenListKhachHang);

                // Delete related records in LuuViec
                var luuViecListKhachHang = db.LuuViecs.Where(lv => lv.MaKH == id).ToList();
                db.LuuViecs.DeleteAllOnSubmit(luuViecListKhachHang);

                var comment=db.Comments.Where(c => c.MaKH==id).ToList();
                db.Comments.DeleteAllOnSubmit(comment);

                // Now delete the KhachHang record
                db.KhachHangs.DeleteOnSubmit(khachHangToDelete);
                db.SubmitChanges();

                return RedirectToAction("Index");
            }
            else
            {
                // Handle the case where KhachHang with the specified ID was not found
                return HttpNotFound();
            }
        }





        //[HttpPost]
        // public ActionResult Xoa(KhachHang  khachhang)
        // {
        // Kiểm tra xem khachhang có tồn tại trong cơ sở dữ liệu không
        //    var xoakhachhang = db.KhachHangs.FirstOrDefault(n => n.MaKH == khachhang.MaKH);

        //      db.KhachHangs.DeleteOnSubmit(xoakhachhang);
        //      db.SubmitChanges();
        //     return RedirectToAction("Index");

        // }

        [HttpGet]
        public ActionResult Sua(int ? id)
        {
            var sua=db.KhachHangs.Where(n=>n.MaKH == id).FirstOrDefault();
            return View(sua);
        }

        [HttpPost]
        public ActionResult Sua(KhachHang khachhang)
        {
            KhachHang ukh = db.KhachHangs.FirstOrDefault(x => x.MaKH == khachhang.MaKH);
            ukh.HoTen = khachhang.HoTen;
            ukh.TaiKhoan = khachhang.TaiKhoan;
            ukh.MatKhau = khachhang.MatKhau;
            ukh.Email = khachhang.Email;         
            ukh.DiaChi = khachhang.DiaChi;
            ukh.DienThoai = khachhang.DienThoai;
            ukh.NgaySinh = khachhang.NgaySinh;
            ukh.IDQuyen = khachhang.IDQuyen;
            db.SubmitChanges();
            return RedirectToAction("Index");
        }
       


    }
}