using System.Collections.Generic;
using ProjectTaskManagement.DAL;
using ProjectTaskManagement.DTO;

namespace ProjectTaskManagement.BUS
{
    public class PhieuSuaChuaBUS
    {
        private PhieuSuaChuaDAL phieuDAL = new PhieuSuaChuaDAL();

        public bool LapPhieuSuaChua(string bienSo, string noiDung, List<ChiTietSuaChuaDTO> dsChiTiet, out int maPhieu, out string errorMsg)
        {
            if (dsChiTiet == null || dsChiTiet.Count == 0)
            {
                maPhieu = 0;
                errorMsg = "Danh sách phụ tùng sửa chữa không được để trống!";
                return false;
            }

            return phieuDAL.LapPhieuSuaChua(bienSo, noiDung, dsChiTiet, out maPhieu, out errorMsg);
        }

        public decimal ThanhToan(int maPhieu)
        {
            return phieuDAL.ThanhToan(maPhieu);
        }
    }
}
