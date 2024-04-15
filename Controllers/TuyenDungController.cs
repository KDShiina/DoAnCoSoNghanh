using DoAnCoSoNghanh.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net;


namespace DoAnCoSoNghanh.Controllers
{
    public class TuyenDungController : Controller
    {
        // GET: TuyenDung
        TimViecDataContext db = new TimViecDataContext();
        public ActionResult Index()
        {
            if (Session["KhachHang"] != null)
            {
                KhachHang khachHang = (KhachHang)Session["KhachHang"];
                var danhSachCongViec = db.ViecLams.Where(vl => vl.MaKH == khachHang.MaKH).ToList();
                return View(danhSachCongViec);
            }
            else
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap", "Home");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["KhachHang"] != null)
            {
                KhachHang khachHang = (KhachHang)Session["KhachHang"];
                NhaTuyenDung nhaTuyenDung = db.NhaTuyenDungs.SingleOrDefault(ntd => ntd.MaKH == khachHang.MaKH);

                if (nhaTuyenDung != null && !string.IsNullOrEmpty(nhaTuyenDung.TenCongTy) && !string.IsNullOrEmpty(nhaTuyenDung.MoTaCongTy))
                {
                    ViewBag.DanhMucList = new SelectList(db.LoaiVieclams, "Mal", "Tenloai");
                    return View();
                }
                else
                {
                    // If information is not updated, redirect to the update information page
                    return RedirectToAction("CapNhatThongTin", "TuyenDung");
                }
            }
            else
            {
                return RedirectToAction("DangNhap", "Home"); 
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(ViecLam vieclam, FormCollection f)
        {
            try
            {
                if (Session["KhachHang"] != null)
                {                  
                    KhachHang khachHang = (KhachHang)Session["KhachHang"];
                    vieclam.MaKH = khachHang.MaKH;

                    NhaTuyenDung nhaTuyenDung = db.NhaTuyenDungs.SingleOrDefault(ntd => ntd.MaKH == khachHang.MaKH);

                    if (nhaTuyenDung != null)
                    {
                        vieclam.MaNTD = nhaTuyenDung.MaNTD;
                    }
                    if (vieclam.NgayHetHan <= vieclam.NgayDang)
                    {
                        ModelState.AddModelError("NgayHetHan", "Ngày hết hạn phải sau ngày đăng.");
                        ViewBag.DanhMucList = new SelectList(db.LoaiVieclams, "Mal", "Tenloai");
                        return View();
                    }
                    var danhSachDonUngTuyen = db.DonUngTuyens.Where(d => d.MaCongViec == vieclam.MaCongViec).ToList();

                    if (vieclam.ImageUpload != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(vieclam.ImageUpload.FileName);
                        string extension = Path.GetExtension(vieclam.ImageUpload.FileName);
                        fileName = fileName + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                        vieclam.Hinh = fileName;
                        vieclam.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
                    }
                    vieclam.QuyenLoiCongViec = f["QuyenLoiCongViec"];
                    vieclam.YeuCauCongViec = f["YeuCauCongViec"];
                    vieclam.MoTaCongViec = f["MoTaCongViec"];
                    vieclam.TrangThai = 0;
                    vieclam.NgayDang = DateTime.Now;

                    db.ViecLams.InsertOnSubmit(vieclam);
                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("DangNhap", "Home");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public ActionResult DangKyNTD()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKyNTD(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                // Thực hiện đăng ký tài khoản Người Tuyển Dụng
                kh.IDQuyen = 2;
                db.KhachHangs.InsertOnSubmit(kh);
                db.SubmitChanges();

                // Lấy MaKH của tài khoản vừa được tạo
                int maKH = kh.MaKH;

                // Tạo một bản ghi mới trong bảng NhaTuyenDung và cập nhật MaKH
                NhaTuyenDung nhaTuyenDung = new NhaTuyenDung
                {
                    MaKH = maKH, // Gán MaKH vừa lấy được từ tài khoản Người Tuyển Dụng

                };

                // Thêm bản ghi vào bảng NhaTuyenDung
                db.NhaTuyenDungs.InsertOnSubmit(nhaTuyenDung);
                db.SubmitChanges();

                // Sau khi cập nhật xong, bạn có thể thực hiện các hành động khác, ví dụ chuyển hướng đến trang chính.

                return RedirectToAction("DangNhap","Home");
            }
            return View(kh);
        }
        public ActionResult CapNhatThongTin()
        {
            if (Session["KhachHang"] != null)
            {
                // Get the current user from the session
                KhachHang khachHang = (KhachHang)Session["KhachHang"];

                // Check if the user is an NhaTuyenDung
                NhaTuyenDung nhaTuyenDung = db.NhaTuyenDungs.SingleOrDefault(ntd => ntd.MaKH == khachHang.MaKH);

                if (nhaTuyenDung != null)
                {
                    return View(nhaTuyenDung);
                }
                else
                {
                    // If the user is not an NhaTuyenDung, you can handle it as needed, for example, by redirecting them to the index page.
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // If the user is not logged in, redirect to the login page or handle it as per your requirements.
                return RedirectToAction("DangNhap", "Home");
            }
        }

        [HttpPost]
        public ActionResult CapNhatThongTin(NhaTuyenDung nhaTuyenDung)
        {
            if (ModelState.IsValid)
            {
                // Check if the user is logged in
                if (Session["KhachHang"] != null)
                {
                    KhachHang khachHang = (KhachHang)Session["KhachHang"];

                    // Check if the user is an NhaTuyenDung
                    NhaTuyenDung existingNhaTuyenDung = db.NhaTuyenDungs.SingleOrDefault(ntd => ntd.MaKH == khachHang.MaKH);

                    if (existingNhaTuyenDung != null)
                    {
                        // Update the NhaTuyenDung's information with the new data
                        existingNhaTuyenDung.TenCongTy = nhaTuyenDung.TenCongTy;
                        existingNhaTuyenDung.MoTaCongTy = nhaTuyenDung.MoTaCongTy;
                        existingNhaTuyenDung.Hinh = nhaTuyenDung.Hinh;

                        // Save the changes to the database
                        db.SubmitChanges();

                        // Redirect to a success page or take any other necessary action
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // If the user is not an NhaTuyenDung, handle it as needed (e.g., redirect to the index page).
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    // If the user is not logged in, redirect to the login page or handle it as per your requirements.
                    return RedirectToAction("DangNhap", "Home");
                }
            }
            // If the model state is not valid, return the view with validation errors.
            return View(nhaTuyenDung);
        }

        public ActionResult ChinhSuaThongTin(int? id)
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
                return RedirectToAction("Index", "TuyenDung");
            }
            else
            {
                // Xử lý trường hợp chưa đăng nhập
                return RedirectToAction("DangNhap", "Home");
            }
        }
        [HttpGet]
        public ActionResult Sua(int? id)
        {
            var sua = db.ViecLams.Where(n => n.MaCongViec == id).FirstOrDefault();
            ViewBag.DanhMucList = new SelectList(db.LoaiVieclams, "Mal", "TenLoai");
            return View(sua);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Sua(ViecLam vieclam, FormCollection f)
        {

            ViecLam uvl = db.ViecLams.FirstOrDefault(x => x.MaCongViec == vieclam.MaCongViec);

           
            if (vieclam.ImageUpload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(vieclam.ImageUpload.FileName);
                string extension = Path.GetExtension(vieclam.ImageUpload.FileName);
                fileName = fileName + "_" + long.Parse(DateTime.Now.ToString("yyyyMMddhhmmss")) + extension;
                vieclam.Hinh = fileName;
                vieclam.ImageUpload.SaveAs(Path.Combine(Server.MapPath("~/img/"), fileName));
            }
            else
            {
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
            uvl.NgayDang=vieclam.NgayDang;
            uvl.NgayHetHan = vieclam.NgayHetHan;
            uvl.MaNTD = vieclam.MaNTD;
            uvl.Mal=vieclam.Mal;

            uvl.MaNTD = vieclam.MaNTD;
            uvl.MaKH = vieclam.MaKH;

            db.SubmitChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Chitiet(int? id)
        {
            var chitietvieclam = db.ViecLams.Where(n => n.MaCongViec == id).FirstOrDefault();
            return View(chitietvieclam);
        }
        public ActionResult Xoa(int? id)
        {
            var luuViecRecords = db.LuuViecs.Where(lv => lv.MaCongViec == id);
            db.LuuViecs.DeleteAllOnSubmit(luuViecRecords);

            var relatedDonUngTuyens = db.DonUngTuyens.Where(d => d.MaCongViec == id);        
            db.DonUngTuyens.DeleteAllOnSubmit(relatedDonUngTuyens);

            ViecLam viecLam = db.ViecLams.FirstOrDefault(n => n.MaCongViec == id);
            db.ViecLams.DeleteOnSubmit(viecLam);
            db.SubmitChanges();

            return RedirectToAction("Index");
        }


        public ActionResult DanhSachDonUngTuyen()
        {
            if (Session["KhachHang"] != null)
            {
                // Kiểm tra xem người dùng đã đăng nhập hay chưa.
                KhachHang khachHang = (KhachHang)Session["KhachHang"];

                // Kiểm tra xem người dùng có phải là NhaTuyenDung hay không.
                NhaTuyenDung existingNhaTuyenDung = db.NhaTuyenDungs.SingleOrDefault(ntd => ntd.MaKH == khachHang.MaKH);

                if (existingNhaTuyenDung != null)
                {
                    // Truy vấn danh sách đơn ứng tuyển thuộc về nhà tuyển dụng
                    var danhSachDonUngTuyen = db.DonUngTuyens.Where(d => d.MaNTD == existingNhaTuyenDung.MaNTD ).ToList();

                    return View(danhSachDonUngTuyen);
                }
                else
                {
                    // Nếu người dùng không phải là NhaTuyenDung, xử lý theo ý muốn, ví dụ chuyển hướng đến trang chính.
                    return RedirectToAction("Index");
                }
            }
            else
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập hoặc xử lý theo ý muốn.
                return RedirectToAction("DangNhap", "Home");
            }
        }

        [HttpGet]
        public ActionResult SuaDon(int? id)
        {
            var chitietdonungtuyen = db.DonUngTuyens.Where(n => n.MaDonUngTuyen == id).FirstOrDefault();
            return View(chitietdonungtuyen);
        }

        [HttpPost]
        public ActionResult SuaDon(DonUngTuyen donungtuyen,FormCollection f)
        {

            DonUngTuyen dut = db.DonUngTuyens.FirstOrDefault(x => x.MaDonUngTuyen == donungtuyen.MaDonUngTuyen);

            dut.MaCongViec = donungtuyen.MaCongViec;
            dut.NgayNop = donungtuyen.NgayNop;
            dut.MaKH = donungtuyen.MaKH;
            dut.TrangThaiDonUngTuyen = donungtuyen.TrangThaiDonUngTuyen;
            dut.MaNTD = donungtuyen.MaNTD;
            dut.TenNguoiUngTuyen = donungtuyen.TenNguoiUngTuyen;
            dut.EmailNguoiUngTuyen = donungtuyen.EmailNguoiUngTuyen;
            dut.NoiDung = donungtuyen.NoiDung;
            dut.FileCV = donungtuyen.FileCV;
            dut.NgayDuKienDenCongTy = donungtuyen.NgayDuKienDenCongTy;
            dut.NoiDungGuiToiKhachHang = donungtuyen.NoiDungGuiToiKhachHang;
            if (donungtuyen.TrangThaiDonUngTuyen == 1 )
            {
              

                // Lấy thông tin khách hàng từ cơ sở dữ liệu
                KhachHang khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == donungtuyen.MaKH);
                ViecLam ntd = db.ViecLams.FirstOrDefault(kh => kh.MaCongViec == donungtuyen.MaCongViec);

                if (khachHang != null)
                {
                    string customerEmail = khachHang.Email;
                    string customerDiaiem = ntd.DiaDiemLamViec;
                    string customertenvieclam = ntd.TenVieclam;
                    string ngaydukien = dut.NgayDuKienDenCongTy?.ToString("dd-MM-yyyy");
                    string noidungui = dut.NoiDungGuiToiKhachHang;


                    // Gửi email thông báo cho khách hàng
                    var fromAddress = new MailAddress("quocthinh5.v2003@gmail.com", "Nhà tuyển dụng");
                    var toAddress = new MailAddress(customerEmail);
                    const string fromPassword = "uuiv yswn atcy yomu";
                    const string subject = "Thông báo kết quả đơn ứng tuyển";
                    string body = $"Chúc mừng! Đơn ứng tuyển vào công việc {customertenvieclam} của bạn đã được chấp nhận. Hãy đến công ty vào ngày {ngaydukien} tại {customerDiaiem} để phỏng vấn nhé! Chúc bạn thành công!\n";
                    body += $"Nội dung gửi tới bạn: {noidungui}";
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
            else if(donungtuyen.TrangThaiDonUngTuyen == 2)
            {
                // Lấy thông tin khách hàng từ cơ sở dữ liệu
                KhachHang khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == donungtuyen.MaKH);
                ViecLam ntd = db.ViecLams.FirstOrDefault(kh => kh.MaCongViec == donungtuyen.MaCongViec);

                if (khachHang != null)
                {
                    string customerEmail = khachHang.Email;
                    string customerDiaiem = ntd.DiaDiemLamViec;
                    string customertenvieclam = ntd.TenVieclam;
                    string noidungui = dut.NoiDungGuiToiKhachHang;

                    // Gửi email thông báo cho khách hàng
                    var fromAddress = new MailAddress("quocthinh5.v2003@gmail.com", "Nhà tuyển dụng");
                    var toAddress = new MailAddress(customerEmail);
                    const string fromPassword = "uuiv yswn atcy yomu";
                    const string subject = "Thông báo kết quả đơn ứng tuyển";
                    string body = $"Cảm ơn bạn đã ứng tuyển vị trí {customertenvieclam} . Chúng tôi xin thông báo rằng đơn ứng tuyển của bạn đã được xem xét và không được chấp nhận. Chúng tôi đánh giá sự quan tâm của bạn. Nếu có nhu cầu, bạn có thể liên hệ để biết thêm thông tin phản hồi. \n";
                    body += $"Nội dung gửi tới bạn: {noidungui}";

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
            return RedirectToAction("DanhSachDonUngTuyen", "TuyenDung");
        }



        [HttpGet]
        public ActionResult Chitietdonungtuyen(int? id)
        {
            var chitietdonungtuyen = db.DonUngTuyens.Where(n => n.MaDonUngTuyen == id).FirstOrDefault();
            return View(chitietdonungtuyen);
        }



        public ActionResult ViewFile(int donUngTuyenId)
        {
            var donUngTuyen = db.DonUngTuyens.FirstOrDefault(d => d.MaDonUngTuyen == donUngTuyenId);

            if (donUngTuyen != null && !string.IsNullOrEmpty(donUngTuyen.FileCV))
            {
                var filePath = Path.Combine(Server.MapPath("~/Upload/"), donUngTuyen.FileCV);

                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string mimeType = MimeMapping.GetMimeMapping(donUngTuyen.FileCV);

                    return File(fileBytes, mimeType);
                }
            }

           
            return RedirectToAction("KhongCoCV","TuyenDung");
        }
        public ActionResult KhongCoCV()
        {
            return View();
        }


      /*  public ActionResult DLKHD()
        {
            if (Session["KhachHang"] == null)
            {
                return RedirectToAction("DangNhap", "Home"); 
            }

            var khachHang = (KhachHang)Session["KhachHang"];
            var tenCongTy = db.NhaTuyenDungs
                        .Where(ntd => ntd.MaKH == khachHang.MaKH)
                        .Select(ntd => ntd.TenCongTy)
                        .FirstOrDefault();
            var viewModel = new DatLichViewModel
            {
                MaKH = khachHang.MaKH,
                HoTen = khachHang.HoTen,
                NgaySinh = (DateTime)khachHang.NgaySinh,
                TenCongTy = tenCongTy,
                DiaChi = khachHang.DiaChi,
                DienThoai = khachHang.DienThoai,
                Email = khachHang.Email,
                NgayDatLich = DateTime.Now 
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult DLKHD(DatLichViewModel viewModel)
        {
            if (ModelState.IsValid) 
            {
                var khachHang = (KhachHang)Session["KhachHang"];

                var nhaTuyenDung = db.NhaTuyenDungs.FirstOrDefault(ntd => ntd.MaKH == khachHang.MaKH);

                if (nhaTuyenDung != null)
                {
                    var datLich = new DatLich
                    {
                        MaNTD = nhaTuyenDung.MaNTD,
                        NgayDatLich = viewModel.NgayDatLich,
                    };

                    db.DatLiches.InsertOnSubmit(datLich);
                    db.SubmitChanges(); 

                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    return RedirectToAction("DangNhap", "Home");
                }
            }
            return View(viewModel);
        }


        // GET: DatLich/Create
        public ActionResult CreateLichHen()
        {
            return View();
        }

        // POST: DatLich/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLichcHen(DatLichViewModel datLichViewModel)
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
        */

    }
}