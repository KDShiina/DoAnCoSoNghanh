using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using DoAnCoSoNghanh.Models;
using PagedList;
using PagedList.Mvc;




namespace DoAnCoSoNghanh.Controllers
{
    public class HomeController : Controller
    {
        TimViecDataContext db = new TimViecDataContext();
        public ActionResult Index(string currentFilter, string SearchString, int? page)
        {
            HomeModels objhome = new HomeModels();

            // Lấy danh sách loại việc làm
            objhome.ListLoaivieclam = db.LoaiVieclams.ToList();

            // Lấy danh sách công việc với điều kiện tìm kiếm
            var listVieclam = db.ViecLams.Where(n => n.TrangThai == 1);

            // Thực hiện tìm kiếm
            if (!string.IsNullOrEmpty(SearchString))
            {
                // Tìm kiếm theo tên công việc hoặc địa điểm làm việc
                listVieclam = listVieclam.Where(v => v.TenVieclam.Contains(SearchString) || v.DiaDiemLamViec.Contains(SearchString));

                // Nếu muốn thêm tìm kiếm theo loại công việc, hãy sử dụng JOIN
                listVieclam = listVieclam
                    .Join(db.LoaiVieclams, v => v.Mal, l => l.Mal, (v, l) => new { Vieclam = v, LoaiVieclam = l })
                    .Where(x => x.Vieclam.TenVieclam.Contains(SearchString) || x.Vieclam.DiaDiemLamViec.Contains(SearchString) || x.LoaiVieclam.TenLoai.Contains(SearchString))
                    .Select(x => x.Vieclam);
            }

            objhome.Listvieclam = listVieclam.OrderByDescending(x => x.MaCongViec).ToList();

            // Lấy danh sách comment
            objhome.ListComment = db.Comments.ToList();

            return View(objhome);
        }


        [HttpPost]
        public ActionResult DangKy(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tài khoản hoặc email đã tồn tại chưa
                if (db.KhachHangs.Any(nd => nd.TaiKhoan == khachHang.TaiKhoan || nd.Email == khachHang.Email))
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản hoặc email đã tồn tại.");
                    return View(khachHang);
                }
                if (Session["KhachHang"] != null)
                {
                    var loggedInUser = Session["KhachHang"] as KhachHang;
                    if (loggedInUser.IDQuyen == 3)
                    {
                        khachHang.IDQuyen = 2;
                    }
                    else
                    {
                        khachHang.IDQuyen = 3;
                    }
                }
                else
                {
                    khachHang.IDQuyen = 3;
                }

                // Thêm người dùng mới vào cơ sở dữ liệu
                db.KhachHangs.InsertOnSubmit(khachHang);
                db.SubmitChanges();

                return RedirectToAction("DangNhap", "Home");
            }


            // Nếu ModelState không hợp lệ, hiển thị lại form đăng ký với thông báo lỗi
            return View(khachHang);
        }

        public ActionResult DangKy()
        {
            return View();
        }


        public ActionResult Apply()
        {
            return View();
        }

        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(KhachHang khachHang,NhaTuyenDung ntd)
        {

            var taikhoan = khachHang.TaiKhoan;
            var matkhau = khachHang.MatKhau;
            var kiemtra = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan.Equals(taikhoan) && x.MatKhau.Equals(matkhau));
            if (kiemtra != null)
            {
                Session["KhachHang"] = kiemtra;
                Session["MaKH"]=kiemtra.MaKH;
                Session["Hoten"] = kiemtra.HoTen;

                if (kiemtra.IDQuyen == 1)
                {
                    return RedirectToAction("Index", "ADMIN");
                }
                else if(kiemtra.IDQuyen == 2)
                {
                    var nhaTuyenDung = db.NhaTuyenDungs.SingleOrDefault(x => x.MaKH == kiemtra.MaKH);
                    if (nhaTuyenDung != null)
                    {
                        Session["MaNTD"] = nhaTuyenDung.MaNTD;
                        return RedirectToAction("Index", "TuyenDung");
                    }
                    else
                    {
                        return RedirectToAction("DangNhap", "Home");
                    }
                }

                else if (kiemtra.IDQuyen == 3)
                {
                    return RedirectToAction("Index", "Home");
                }
               
            }
            else
            {
                ViewBag.LoiDangNhap = "Thông tin đăng nhập sai, vui lòng kiểm tra lại !!.";
                return View("DangNhap");
            }
            return View();
        }
        public ActionResult DangXuat()
        {
            Session["KhachHang"] = null;
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ThongTinCaNhan()
        {
            // Lấy thông tin cá nhân từ session (đã đăng nhập)
            KhachHang khachHang = Session["KhachHang"] as KhachHang;

            if (khachHang != null)
            {
                return View(khachHang);
            }
            else
            {
                // Xử lý trường hợp chưa đăng nhập
                return RedirectToAction("DangNhap");
            }
        }
        public ActionResult ChinhSuaThongTin(int ? id)
        {
            KhachHang khachHang = Session["KhachHang"] as KhachHang;

            if (khachHang != null)
            {
                KhachHang kh = db.KhachHangs.FirstOrDefault(x => x.MaKH == id);
                return View(khachHang);
            }
            else
            {
                // Xử lý trường hợp chưa đăng nhập
                return RedirectToAction("DangNhap");
            }

        }

        [HttpPost]
        public ActionResult ChinhSuaThongTin(KhachHang khachHang)
        {
            KhachHang loggedInUser = Session["KhachHang"] as KhachHang;

            if (loggedInUser != null)
            {
                KhachHang ukh = db.KhachHangs.FirstOrDefault(x => x.MaKH == khachHang.MaKH);
                // Cập nhật thông tin người dùng trong cơ sở dữ liệu
                ukh.HoTen = khachHang.HoTen;
                ukh.Email = khachHang.Email;
                ukh.MatKhau = khachHang.MatKhau;
                ukh.DiaChi = khachHang.DiaChi;
                ukh.DienThoai = khachHang.DienThoai;
                ukh.NgaySinh = khachHang.NgaySinh;

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SubmitChanges();

                // Cập nhật thông tin người dùng trong Session
                Session["KhachHang"] = ukh;

                // Redirect to the user's profile page
                return RedirectToAction("ThongTinCaNhan");
            }
            else
            {
                // Xử lý trường hợp chưa đăng nhập
                return RedirectToAction("DangNhap");
            }
        }
        public ActionResult DanhSachDonUngTuyen()
        {
            if (Session["KhachHang"] != null)
            {
                // Kiểm tra xem khách hàng đã đăng nhập hay chưa.
                KhachHang khachHang = (KhachHang)Session["KhachHang"];

                // Truy vấn danh sách đơn ứng tuyển của khách hàng dựa trên MaKH
                var danhSachDonUngTuyen = db.DonUngTuyens.Where(d => d.MaKH == khachHang.MaKH).ToList();

                if (danhSachDonUngTuyen.Count == 0)
                {
                    TempData["Message"] = "Bạn chưa ứng tuyển công việc nào.";
                }
                return View(danhSachDonUngTuyen);
            }
            else
            {
                // Nếu khách hàng chưa đăng nhập, chuyển hướng đến trang đăng nhập hoặc xử lý theo ý muốn.
                return RedirectToAction("DangNhap");
            }
        }

        public ActionResult XoaDonUngTuyen(int id)
        {
    
            if (Session["KhachHang"] != null)
            {
                // Lấy thông tin người dùng từ Session
                KhachHang khachHang = (KhachHang)Session["KhachHang"];

                // Kiểm tra xem đơn ứng tuyển có thuộc về người dùng hiện tại hay không.
                DonUngTuyen donUngTuyen = db.DonUngTuyens.FirstOrDefault(d => d.MaDonUngTuyen == id && d.MaKH == khachHang.MaKH);
                if (donUngTuyen != null)
                {
                    // Xóa đơn ứng tuyển khỏi cơ sở dữ liệu.
                    db.DonUngTuyens.DeleteOnSubmit(donUngTuyen);
                    db.SubmitChanges();
                }
            }
            return RedirectToAction("Index","Home");
        }
        public ActionResult SaveJob(int id)
        {
            if (Session["KhachHang"] != null)
            {
                // Get the logged-in user
                KhachHang khachHang = Session["KhachHang"] as KhachHang;
                var job = db.ViecLams.FirstOrDefault(v => v.MaCongViec == id);
                if (job != null)
                {
                    // Check if the job is not already saved by the user
                    if (!db.LuuViecs.Any(l => l.MaKH == khachHang.MaKH && l.MaCongViec == id))
                    {
                        // Save the job for the user
                        LuuViec luuViec = new LuuViec
                        {
                            MaKH = khachHang.MaKH,
                            MaCongViec = id,
                            MaNTD = job.MaNTD

                        };

                        db.LuuViecs.InsertOnSubmit(luuViec);
                        db.SubmitChanges();
                    }
                }
            }
            else
            {
                return RedirectToAction("DangNhap","Home");

            }

            // Redirect back to the job listing page
            return RedirectToAction("Index");
        }
        public ActionResult SavedJobs()
        {
            if (Session["KhachHang"] != null)
            {
                KhachHang khachHang = (KhachHang)Session["KhachHang"];
                int maKH = khachHang.MaKH;

                // Query the database to get the saved jobs for the current user
                var savedJobs = db.LuuViecs
                    .Where(lv => lv.MaKH == maKH)
                    .Select(lv => lv.ViecLam)
                    .ToList();
                if (savedJobs.Count == 0)
                {
                    TempData["Message"] = "Bạn chưa lưu công việc nào.";
                }

                return View(savedJobs);
            }
            else
            {
                return RedirectToAction("DangNhap"); // Redirect to login if the user is not logged in
            }
        }
        public ActionResult XoaSaveJobs(int id)
        {
            if (Session["KhachHang"] != null)
            {
                // Get the logged-in user
                KhachHang khachHang = Session["KhachHang"] as KhachHang;

                // Find the saved job entry in the database
                LuuViec luuViec = db.LuuViecs.FirstOrDefault(lv => lv.MaKH == khachHang.MaKH && lv.MaCongViec == id);

                if (luuViec != null)
                {
                    // Remove the saved job entry from the database
                    db.LuuViecs.DeleteOnSubmit(luuViec);
                    db.SubmitChanges();
                }
            }

            // Redirect back to the saved jobs listing page
            return RedirectToAction("SavedJobs");
        }


        public ActionResult Danhmuc()
        {
            var lstloaidanhmuc = db.LoaiVieclams.ToList(); 

            return View(lstloaidanhmuc);
        }

        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuenMatKhau(quenmatkhau model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email có tồn tại trong cơ sở dữ liệu hay không
                var user = db.KhachHangs.SingleOrDefault(u => u.Email == model.Email);

                if (user != null)
                {
                    // Gửi mật khẩu cũ đến địa chỉ email của người dùng
                    SendPasswordByEmail(user.Email, user.MatKhau);

                    ViewBag.Message = "Mật khẩu đã được gửi đến địa chỉ email của bạn.";
                    return RedirectToAction("DangNhap", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email không tồn tại.");
                }
            }
            return View(model);
        }

        private void SendPasswordByEmail(string email, string password)
        {
           
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("quocthinh5.v2003@gmail.com", "uuiv yswn atcy yomu"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("quocthinh5.v2003@gmail.com"),
                Subject = "Mật khẩu của bạn",
                Body = "Mật khẩu của bạn là: " + password,
            };

            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
        }

        public ActionResult CommentsByUser(int userId)
        {
            var comments = db.Comments.Where(c => c.MaKH == userId).ToList(); // Lấy tất cả các comment của một người dùng từ database
            var homeModel = new HomeModels
            {
                ListComment = comments ?? new List<Comment>() // Initialize ListComment with comments or an empty list if comments is null
            };
            return View(homeModel); // Trả về view và truyền đối tượng HomeModels chứa danh sách comment cho view
        }
    }
}