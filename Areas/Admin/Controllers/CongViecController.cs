using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DoAnCoSoNghanh.Models;
using PagedList;
using PagedList.Mvc;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class CongViecController : Controller
    {
        TimViecDataContext db = new TimViecDataContext();
        // GET: Admin/CongViec
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

                var listViecLam = new List<ViecLam>();
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
                    listViecLam = db.ViecLams.Where(n => n.TenVieclam.Contains(SearchString)).ToList();
                }
                else
                {
                    listViecLam = db.ViecLams.ToList();

                }
                ViewBag.CurrentFilter = SearchString;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                listViecLam = listViecLam.OrderByDescending(n => n.MaCongViec).ToList();
                return View(listViecLam.ToPagedList(pageNumber, pageSize));
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
            var chitietvieclam = db.ViecLams.Where(n => n.MaCongViec == id).FirstOrDefault();
            return View(chitietvieclam);
        }

        [HttpGet]
        public ActionResult Them()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Them(ViecLam vieclam)
        {
            try
            {
                if (vieclam.ImageUpload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(vieclam.ImageUpload.FileName);
                    string extension = Path.GetExtension(vieclam.ImageUpload.FileName);
                    fileName = fileName + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                    vieclam.Hinh = fileName;
                    vieclam.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                }
                db.ViecLams.InsertOnSubmit(vieclam);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Sua(int? id)
        {
            var sua = db.ViecLams.Where(n => n.MaCongViec == id).FirstOrDefault();
            return View(sua);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Sua(ViecLam vieclam,FormCollection f)
        {
            ViecLam uvl = db.ViecLams.FirstOrDefault(x => x.MaCongViec == vieclam.MaCongViec);
            if (vieclam.ImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(vieclam.ImageUpload.FileName);
                string extension = Path.GetExtension(vieclam.ImageUpload.FileName);
                fileName = fileName + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                vieclam.Hinh = fileName;
                vieclam.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
            }else
            {
                // Nếu không có ảnh mới được tải lên, giữ nguyên ảnh hiện tại
                vieclam.Hinh = uvl.Hinh;
            }
          
            uvl.Hinh = vieclam.Hinh;
            uvl.TenVieclam = vieclam.TenVieclam;
            uvl.DiaDiemLamViec = vieclam.DiaDiemLamViec;
            uvl.LoaiViecLam = vieclam.LoaiViecLam;
            uvl.Luong = vieclam.Luong;
            uvl.MoTaCongViec = f["MoTaCongViec"];
            uvl.YeuCauCongViec = f["YeuCauCongViec"];
            uvl.QuyenLoiCongViec = f["QuyenLoiCongViec"];
            uvl.NgayHetHan = vieclam.NgayHetHan;
            uvl.MaNTD = vieclam.MaNTD;
            uvl.TrangThai = vieclam.TrangThai;
            uvl.MaNTD = vieclam.MaNTD;
            uvl.MaKH = vieclam.MaKH;
            if (vieclam.TrangThai == 1)
            {
                // Lấy thông tin khách hàng từ cơ sở dữ liệu
                KhachHang khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == vieclam.MaKH);
                ViecLam ntd = db.ViecLams.FirstOrDefault(kh => kh.MaCongViec == vieclam.MaCongViec);

                if (khachHang != null)
                {
                    string customerEmail = khachHang.Email;
                    string customerDiaiem = ntd.DiaDiemLamViec;

                    // Gửi email thông báo cho khách hàng
                    var fromAddress = new MailAddress("quocthinh5.v2003@gmail.com", "Admin");
                    var toAddress = new MailAddress(customerEmail);
                    const string fromPassword = "uuiv yswn atcy yomu";
                    const string subject = "Thành công bước đầu! Việc làm của bạn đã được xác nhận và sẵn sàng thu hút sự quan tâm ";
                    string body = $"Xin chúc mừng! Việc làm của bạn đã được xác nhận và xuất hiện trên trang web của chúng tôi.";

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
                }
            }
            else if (vieclam.TrangThai == 2)
            {
                // Lấy thông tin khách hàng từ cơ sở dữ liệu
                KhachHang khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == vieclam.MaKH);
                ViecLam ntd = db.ViecLams.FirstOrDefault(kh => kh.MaCongViec == vieclam.MaCongViec);

                if (khachHang != null)
                {
                    string customerEmail = khachHang.Email;
                    string customerDiaiem = ntd.DiaDiemLamViec;

                    // Gửi email thông báo cho khách hàng
                    var fromAddress = new MailAddress("quocthinh5.v2003@gmail.com", "Admin");
                    var toAddress = new MailAddress(customerEmail);
                    const string fromPassword = "uuiv yswn atcy yomu";
                    const string subject = "Thông báo về việc làm của bạn";
                    string body = $"Xin lỗi, thông báo rằng việc làm của bạn đã bị từ chối. Hãy kiểm tra và cập nhật thông tin để đảm bảo tuân thủ các quy định của chúng tôi.\n\n";
                    body += "Lý do từ chối:\n";
                    body += "1. **Thông tin không đầy đủ:** Đảm bảo bạn đã nhập đầy đủ thông tin chi tiết về công việc.\n";
                    body += "2. **Hình ảnh không phù hợp:** Hình ảnh liên quan đến công việc làm rất quan trọng. Hãy đảm bảo bạn đã thêm hình ảnh chất lượng và phù hợp.\n";
                    body += "3. **Mô tả công việc không rõ ràng:** Mô tả công việc cần phải mô tả rõ ràng về nhiệm vụ, yêu cầu, và quyền lợi để thu hút ứng viên.\n\n";
                    body += "Nếu có bất kỳ câu hỏi hoặc cần hỗ trợ, vui lòng liên hệ với chúng tôi qua email: quoccthinhh03@gmail.com.\n";
                    body += "Xin cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.\n";
                    body += "Trân trọng,\n";
                    body += "Đội ngũ hỗ trợ của chúng tôi";

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
                }
            }
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Xoa(int? id)
        {
            var donUngTuyenRecords = db.DonUngTuyens.Where(d => d.MaCongViec == id);

            foreach (var donUngTuyen in donUngTuyenRecords)
            {
                db.DonUngTuyens.DeleteOnSubmit(donUngTuyen);
            }

            var donUngTuyenRecordss = db.LuuViecs.Where(d => d.MaCongViec == id);

            foreach (var donUngTuyen in donUngTuyenRecordss)
            {
                db.LuuViecs.DeleteOnSubmit(donUngTuyen);
            }

            var viecLam = db.ViecLams.FirstOrDefault(v => v.MaCongViec == id);
            db.ViecLams.DeleteOnSubmit(viecLam);

            db.SubmitChanges();

            return RedirectToAction("Index");
        }



    }
}