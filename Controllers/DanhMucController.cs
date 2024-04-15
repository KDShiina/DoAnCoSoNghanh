using DoAnCoSoNghanh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnCoSoNghanh.Controllers
{
    public class DanhMucController : Controller
    {
        // GET: DanhMuc
        TimViecDataContext db = new TimViecDataContext();
        public ActionResult Index()
        {
            var lstdanhmuc = db.LoaiVieclams.ToList();
            return View(lstdanhmuc);
        }

        public ActionResult DanhMucCongViec(int Id)
        {
            var lstDanhMuc = db.ViecLams.Where(n => n.Mal == Id && n.TrangThai==1).ToList();
            return View(lstDanhMuc);
        }
    }
}