using ProjectTaskManagement.DAL;
using ProjectTaskManagement.DTO;

namespace ProjectTaskManagement.BUS
{
    public class XeBUS
    {
        private XeDAL xeDAL = new XeDAL();

        public bool KiemTraXeTonTai(string bienSo)
        {
            if (string.IsNullOrWhiteSpace(bienSo)) return false;
            return xeDAL.KiemTraXeTonTai(bienSo.Trim());
        }

        public bool TiepNhanXeMoi(KhachHangDTO kh, XeDTO xe)
        {
            if (string.IsNullOrWhiteSpace(xe.BienSo) || string.IsNullOrWhiteSpace(kh.TenKH))
                return false;

            return xeDAL.ThemKhachHangVaXe(kh, xe);
        }
    }
}
