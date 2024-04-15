using DoAnCoSoNghanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class QuanLyLichHenController : Controller
    {
        TimViecDataContext db = new TimViecDataContext();

        public ActionResult Index()
        {
            var query = from datLich in db.DatLiches
                        join nhaTuyenDung in db.NhaTuyenDungs on datLich.MaNTD equals nhaTuyenDung.MaNTD
                        join khachHang in db.KhachHangs on nhaTuyenDung.MaKH equals khachHang.MaKH
                        select new DatLichViewModel
                        {
                            MaKH = khachHang.MaKH,
                            TenCongTy = nhaTuyenDung.TenCongTy,
                            NgayDatLich = (DateTime)datLich.NgayDatLich,
                            HoTen = khachHang.HoTen
                        };

            var lichHens = query.ToList();

            return View(lichHens);
        }


        // GET: DatLich/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DatLich/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DatLichViewModel datLichViewModel)
        {
            if (ModelState.IsValid)
            {
                // Tạo một đối tượng mới của lịch hẹn từ dữ liệu trong ViewModel
                var datLich = new DatLich
                {
                    MaDatLich = datLichViewModel.MaDatLich,
                    MaNTD = datLichViewModel.MaNTD,
                    NgayDatLich = datLichViewModel.NgayDatLich,
                    // Các trường dữ liệu khác
                };

                // Lưu lịch hẹn vào cơ sở dữ liệu
                db.DatLiches.InsertOnSubmit(datLich);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            return View(datLichViewModel);
        }

/*        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(int maDatLich, string emailKhachHang)
        {
            var datLich = db.DatLiches.Find(maDatLich);
            if (datLich == null)
            {
                return HttpNotFound();
            }
            try
            {
                var fromAddress = new MailAddress("your-email@gmail.com", "Your Name");
                var toAddress = new MailAddress(emailKhachHang, "Customer Name");
                const string fromPassword = "your-email-password";
                const string subject = "Xác nhận lịch hẹn";
                string body = "Chúng tôi đã nhận được yêu cầu xác nhận lịch hẹn của bạn.";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                ViewBag.Message = "Email xác nhận đã được gửi thành công.";
            }
            catch (Exception)
            {
                ViewBag.Error = "Có lỗi xảy ra khi gửi email xác nhận.";
            }

            return View();
        }
*/    }
}
