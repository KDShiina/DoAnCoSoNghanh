using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DoAnCoSoNghanh.Models
{
    public class quenmatkhau
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}