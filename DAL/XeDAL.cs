using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ProjectTaskManagement.DTO;

namespace ProjectTaskManagement.DAL
{
    public class XeDAL : DatabaseAccess
    {
        public bool KiemTraXeTonTai(string bienSo)
        {
            using (SqlConnection conn = GetConnection())
            {
                string query = "SELECT COUNT(1) FROM Xe WHERE BienSo = @BienSo";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add("@BienSo", SqlDbType.VarChar, 20).Value = bienSo;

                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        public bool ThemKhachHangVaXe(KhachHangDTO kh, XeDTO xe)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlKH = @"INSERT INTO KhachHang (TenKH, DienThoai, DiaChi) 
                                         VALUES (@TenKH, @DienThoai, @DiaChi); 
                                         SELECT SCOPE_IDENTITY();";
                        SqlCommand cmdKH = new SqlCommand(sqlKH, conn, trans);
                        cmdKH.Parameters.AddWithValue("@TenKH", kh.TenKH);
                        cmdKH.Parameters.AddWithValue("@DienThoai", kh.DienThoai ?? (object)DBNull.Value);
                        cmdKH.Parameters.AddWithValue("@DiaChi", kh.DiaChi ?? (object)DBNull.Value);

                        int maKH = Convert.ToInt32(cmdKH.ExecuteScalar());

                        string sqlXe = @"INSERT INTO Xe (BienSo, MaKH, HieuXe, NamSanXuat) 
                                         VALUES (@BienSo, @MaKH, @HieuXe, @NamSanXuat);";
                        SqlCommand cmdXe = new SqlCommand(sqlXe, conn, trans);
                        cmdXe.Parameters.AddWithValue("@BienSo", xe.BienSo);
                        cmdXe.Parameters.AddWithValue("@MaKH", maKH);
                        cmdXe.Parameters.AddWithValue("@HieuXe", xe.HieuXe ?? (object)DBNull.Value);
                        cmdXe.Parameters.AddWithValue("@NamSanXuat", xe.NamSanXuat);

                        cmdXe.ExecuteNonQuery();
                        trans.Commit();
                        return true;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
