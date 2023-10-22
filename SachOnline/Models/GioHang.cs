using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SachOnline.Models
{
    public partial class GioHang
    {
        dbSachOnlineDataContext dbSachOnlineDataContext;
        public int iSachID { get; set; }
        public string sTenSach { get; set; }
        public string sAnhSP { get; set; }
        public double dGiaTien { get; set; }
        public int iSoLuong { get; set; }
        public double dTongTien
        {
            get { return iSoLuong * dGiaTien; }

        }

        public GioHang(int ms)
        {
            iSachID = ms;
            dbSachOnlineDataContext = new dbSachOnlineDataContext("Data Source=DESKTOP-8DF9EK9\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True");

            SACH s = dbSachOnlineDataContext.SACHes.Single(n => n.SachID == iSachID);
            sTenSach = s.TenSach;
            sAnhSP = s.anhSP;
            dGiaTien = double.Parse(s.GiaBan.ToString());
            iSoLuong = 1;
        }

    }
}