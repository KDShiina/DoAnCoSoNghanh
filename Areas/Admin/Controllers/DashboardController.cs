using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnCoSoNghanh.Models;

namespace DoAnCoSoNghanh.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        TimViecDataContext db = new TimViecDataContext();
       
        public ActionResult Dashboard()
        {

            return View();
        }
        public ActionResult DangNhap()
        {
            return View();
        }
        

    }
}