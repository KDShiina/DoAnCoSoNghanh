using DoAnCoSoNghanh.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace DoAnCoSoNghanh.Controllers
{
    public class JobDetailsController : Controller
    {
        TimViecDataContext db = new TimViecDataContext();
        // GET: JobDetails
        public ActionResult Index(int? Id)
        {
            var lstdetails = db.ViecLams.Where(n => n.MaCongViec == Id).FirstOrDefault();
            return View(lstdetails);
        }

        [HttpPost]
        public ActionResult ApplyForJob(int ?MaKH, int ?MaCongViec, HttpPostedFileBase file, string TenNguoiUngTuyen, string EmailNguoiUngTuyen,string NoiDung)
        {
           
            KhachHang khachHang = (KhachHang)Session["KhachHang"];
            // Create a new entry in the DonUngTuyen table using MaKH and MaCongViec
            var job = db.ViecLams.FirstOrDefault(v => v.MaCongViec == MaCongViec);
            if (job != null)
            {
                // Tạo một bản ghi mới trong bảng DonUngTuyen và gán mantd từ công việc
                DonUngTuyen newApplication = new DonUngTuyen
                {
                    MaKH = khachHang.MaKH,
                    MaCongViec = MaCongViec,
                    MaNTD = job.MaNTD, // Lưu mantd của công việc
                    TrangThaiDonUngTuyen = 0,
                    TenNguoiUngTuyen=TenNguoiUngTuyen,
                    EmailNguoiUngTuyen=EmailNguoiUngTuyen,
                    NoiDung= NoiDung,
                    NgayNop = DateTime.Now,
                    FileCV = null
                };

                db.DonUngTuyens.InsertOnSubmit(newApplication);
                db.SubmitChanges();
                if (file != null && file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(Server.MapPath("~/Upload/"), fileName);
                    file.SaveAs(filePath);

                    // Update the file path in the database
                    newApplication.FileCV = fileName;
                    db.SubmitChanges();
                }
            }
            return RedirectToAction("ApplyJobSuccess", "JobDetails"); 
        }

        // Dispose the DbContext to release resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult ApplyJobSuccess()
        {
            return View();
        }

    }
}