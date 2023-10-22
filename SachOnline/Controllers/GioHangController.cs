using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SachOnline.Models;

namespace SachOnline.Controllers
{
    public class GioHangController : Controller
    {
        dbSachOnlineDataContext dbSachOnlineDataContext;

        public GioHangController()
        {
            dbSachOnlineDataContext = new dbSachOnlineDataContext("Data Source=DESKTOP-8DF9EK9\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True");
        }

        public ActionResult ThemGioHang(int ms, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Find(n => n.iSachID == ms);
            if (sp == null)
            {
                sp = new GioHang(ms);
                lstGioHang.Add(sp);

                // Đặt TempData với thông báo thêm thành công
                TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào giỏ hàng.";
            }
            else
            {
                sp.iSoLuong++;
            }

            return Redirect(url);
        }


        private List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGIoHang = Session["GioHang"] as List<GioHang>;
            if (lstGIoHang != null)
            {
                iTongSoLuong = lstGIoHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.dTongTien);
            }
            return dTongTien;
        }

        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "SachOnline");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        //Tạo partial view để hiển thị thông tin giỏ hàng 
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }

        //Xóa sản phẩm khỏi giỏ hàng
        public ActionResult XoaSPKhoiGioHang(int iSachID)
        {
            List<GioHang> lstGioHang = LayGioHang();
            //Kiem tra Sach da co trong Session["GioHang"]
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iSachID == iSachID);
            //Xóa sp khỏi giỏ hàng
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iSachID == iSachID);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "SachOnline");
                } 
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapNhatGioHang(int iSachID, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n=>n.iSachID==iSachID);
            if (sp != null)
            {
                sp.iSoLuong= int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang= LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "SachOnline");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"]==null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "SachOnline");
            }

            if (Session["GioHang"]==null)
            {
                return RedirectToAction("Index", "SachOnline");
            }

            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong=TongSoLuong();
            ViewBag.TongTien=TongTien();
            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = new KHACHHANG();
            List<GioHang> lstGioHang = LayGioHang();
            ddh.KhachHangID=kh.KhachHangID;
            ddh.NgayDat=DateTime.Now;
            var NgayGiao = String.Format("{0:MM/dd/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrangDonHang = false;
            ddh.DaThanhToan = false;
            dbSachOnlineDataContext.DONDATHANGs.InsertOnSubmit(ddh);
            dbSachOnlineDataContext.SubmitChanges();

            foreach(var item in lstGioHang)
            {
                CHITIETDATHANG ch = new CHITIETDATHANG();
                ch.DonDatHangID=ddh.DonDatHangID;
                ch.SachID = item.iSachID;
                ch.SoLuong = item.iSoLuong;
                ch.GiaTien = item.dGiaTien;
                dbSachOnlineDataContext.CHITIETDATHANGs.InsertOnSubmit(ch);
            }
            dbSachOnlineDataContext.SubmitChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}
