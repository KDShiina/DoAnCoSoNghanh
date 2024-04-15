using DoAnCoSoNghanh.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class QuanlycommentsController : Controller
    {
        // GET: Admin/Quanlycomments
        TimViecDataContext db= new TimViecDataContext();
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

                var listViecLam = new List<Comment>();
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
                    listViecLam = db.Comments.Where(n => n.Content.Contains(SearchString)).ToList();
                }
                else
                {
                    listViecLam = db.Comments.ToList();

                }
                ViewBag.CurrentFilter = SearchString;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                listViecLam = listViecLam.OrderByDescending(n => n.CommentID).ToList();
                return View(listViecLam.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập.
                return RedirectToAction("DangNhap", "Home", new { area = "" });
            }

            
        }
        public ActionResult Xoa(int? id)
        {
            var comment = db.Comments.FirstOrDefault(v => v.CommentID == id);
            db.Comments.DeleteOnSubmit(comment);

            db.SubmitChanges();

            return RedirectToAction("Index");
        }

    }
}