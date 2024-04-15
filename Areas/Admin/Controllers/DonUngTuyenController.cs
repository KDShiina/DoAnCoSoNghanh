using DoAnCoSoNghanh.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class DonUngTuyenController : Controller
    {
        // GET: Admin/DonUngTuyen
        TimViecDataContext db=new TimViecDataContext();
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

                var listDonungtuyen = new List<DonUngTuyen>();
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
                    listDonungtuyen = db.DonUngTuyens.Where(n => n.MaKH.ToString().Contains(SearchString)).ToList();
                }
                else
                {
                    listDonungtuyen = db.DonUngTuyens.ToList();

                }
                ViewBag.CurrentFilter = SearchString;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                listDonungtuyen = listDonungtuyen.OrderByDescending(n => n.MaDonUngTuyen).ToList();
                return View(listDonungtuyen.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập.
                return RedirectToAction("DangNhap", "Home", new { area = "" });
            }
        }

            [HttpGet]
        public ActionResult Chitiet(int? id)
        {
            var chitietDonungtuyen = db.DonUngTuyens.Where(n => n.MaDonUngTuyen == id).FirstOrDefault();
            return View(chitietDonungtuyen);
        }
        public ActionResult CountDonUngTuyen()
        {
            var count = db.DonUngTuyens.Count();
            ViewBag.CountDonUngTuyen = count;
            return View();
        }
    }
}