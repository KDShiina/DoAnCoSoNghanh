using DoAnCoSoNghanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class ADMINController : Controller
    {
        // GET: Admin/ADMIN
        TimViecDataContext db=new TimViecDataContext();

        public ActionResult Index()
        {
            if (Session["KhachHang"] != null)
            {
                KhachHang khachHang = (KhachHang)Session["KhachHang"];
                if (khachHang.IDQuyen == 1)
                {
                    // User has quyen = 1, allow access to the admin section
                    return View();
                }
                else
                {
                    return  RedirectToAction("DangNhap", "Home", new { area = "" });
                }
            }
            else
            {
                // Nếu không có Session "KhachHang", chuyển hướng người dùng đến trang đăng nhập hoặc thông báo lỗi.
                return RedirectToAction("DangNhap", "Home", new { area = "" }); // Chuyển hướng đến trang đăng nhập hoặc điều chỉnh theo ý muốn.
            }
        }

        // Action DangXuat để đăng xuất
        public ActionResult DangXuat()
        {
            return RedirectToAction("DangNhap", "Home", new { area = "" });
        }
    }
}