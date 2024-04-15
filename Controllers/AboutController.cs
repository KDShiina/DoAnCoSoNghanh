using DoAnCoSoNghanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSoNghanh.Controllers
{
    public class AboutController : Controller
    {
        TimViecDataContext db = new TimViecDataContext();

        // GET: About
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SaveComment(string comment)
        {
            int? userId = GetCurrentUserId();

            if (userId.HasValue)
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    Comment newComment = new Comment
                    {
                        MaKH = userId.Value,
                        Content = comment,
                        CommentTime = DateTime.Now
                    };

                    db.Comments.InsertOnSubmit(newComment);
                    db.SubmitChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Message"] = "Vui lòng đăng nhập để thêm comment.";
                return RedirectToAction("DangNhap", "Home");
            }
        }

        private int? GetCurrentUserId()
        {
            int? maKH = Session["MaKH"] as int?;
            return maKH;
        }

        public ActionResult MyComments()
        {
            int? userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                // Lấy tất cả các comment của người dùng hiện tại
                var userComments = db.Comments.Where(c => c.MaKH == userId.Value).ToList();
                if (userComments.Count == 0)
                {
                    TempData["Message"] = "Bạn chưa đăng bất kỳ bình luận nào.";
                }
                return View(userComments);
            }
            else
            {
                TempData["Message"] = "Vui lòng đăng nhập để xem comment.";
                return RedirectToAction("DangNhap", "Home");
            }
        }
        public ActionResult XoaComment(int id)
        {
            if (Session["MaKH"] != null)
            {
                int? maKH = Session["MaKH"] as int?;

                if (maKH.HasValue)
                {
                    
                    Comment comment = db.Comments.FirstOrDefault(d => d.CommentID == id && d.MaKH == maKH.Value);
                    if (comment != null)
                    {
                        
                        db.Comments.DeleteOnSubmit(comment);
                        db.SubmitChanges();
                    }
                }
            }
            return RedirectToAction("MyComments", "About");
        }

        public ActionResult Sua(int id)
        {
            int? userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                Comment comment = db.Comments.FirstOrDefault(c => c.CommentID == id && c.MaKH == userId.Value);
                if (comment != null)
                {
                    return View(comment);
                }
                else
                {
                    TempData["Message"] = "Không tìm thấy bình luận để chỉnh sửa.";
                    return RedirectToAction("MyComments");
                }
            }
            else
            {
                TempData["Message"] = "Vui lòng đăng nhập để chỉnh sửa bình luận.";
                return RedirectToAction("DangNhap", "Home");
            }
        }

        [HttpPost]
        public ActionResult Sua(int id, string content)
        {
            int? userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                Comment comment = db.Comments.FirstOrDefault(c => c.CommentID == id && c.MaKH == userId.Value);
                if (comment != null)
                {
                    comment.Content = content;
                    comment.CommentTime = DateTime.Now;
                    db.SubmitChanges();
                    TempData["Message"] = "Bình luận đã được chỉnh sửa thành công.";
                    return RedirectToAction("MyComments");
                }
                else
                {
                    TempData["Message"] = "Không tìm thấy bình luận để chỉnh sửa.";
                    return RedirectToAction("MyComments");
                }
            }
            else
            {
                TempData["Message"] = "Vui lòng đăng nhập để chỉnh sửa bình luận.";
                return RedirectToAction("DangNhap", "Home");
            }
        }

    }
}