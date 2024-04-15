using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnCoSoNghanh.Models
{
    public class HomeModels
    {
        public List<NhaTuyenDung> Listnhatuyendung { get; set; }
        public List<ViecLam> Listvieclam { get; set; }
        public List<LoaiVieclam> ListLoaivieclam { get; set; }
        public List<Comment> ListComment { get; set; }


    }
}