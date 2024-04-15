using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCoSoNghanh.Models
{
    public class DatLichViewModel
    {
        public int MaDatLich { get; set; }

        public int MaKH { get; set; }
        public int MaNTD { get; set; }
        public string HoTen { get; set; }
        public DateTime NgaySinh { get; set; }
        public string TenCongTy { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public DateTime NgayDatLich { get; set; }
    }

}