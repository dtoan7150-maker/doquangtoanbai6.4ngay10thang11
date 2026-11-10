using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ProjectTaskManagement.DTO;

namespace ProjectTaskManagement.DAL
{
    public class PhieuSuaChuaDAL : DatabaseAccess
    {
        public bool LapPhieuSuaChua(string bienSo, string noiDungSua, List<ChiTietSuaChuaDTO> dsChiTiet, out int maPhieuTao, out string errorMsg)
        {
            errorMsg = string.Empty;
            maPhieuTao = 0;

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    string sqlPhieu = @"INSERT INTO PhieuSuaChua (BienSo, NoiDungSua, TrangThai) 
                                        VALUES (@BienSo, @NoiDungSua, N'Đang sửa');
                                        SELECT SCOPE_IDENTITY();";
                    SqlCommand cmdPhieu = new SqlCommand(sqlPhieu, conn, trans);
                    cmdPhieu.Parameters.AddWithValue("@BienSo", bienSo);
                    cmdPhieu.Parameters.AddWithValue("@NoiDungSua", noiDungSua);

                    maPhieuTao = Convert.ToInt32(cmdPhieu.ExecuteScalar());

                    foreach (var item in dsChiTiet)
                    {
                        string sqlUpdateKho = @"UPDATE PhuTung 
                                                SET SoLuongTon = SoLuongTon - @SoLuong 
                                                WHERE MaPhuTung = @MaPhuTung AND SoLuongTon >= @SoLuong;";
                        SqlCommand cmdKho = new SqlCommand(sqlUpdateKho, conn, trans);
                        cmdKho.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                        cmdKho.Parameters.AddWithValue("@MaPhuTung", item.MaPhuTung);

                        int rowsAffected = cmdKho.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            trans.Rollback();
                            errorMsg = $"Phụ tùng mã #{item.MaPhuTung} không đủ số lượng tồn trong kho!";
                            return false;
                        }

                        string sqlChiTiet = @"INSERT INTO ChiTietSuaChua (MaPhieu, MaPhuTung, SoLuong, TienCong) 
                                              VALUES (@MaPhieu, @MaPhuTung, @SoLuong, @TienCong);";
                        SqlCommand cmdChiTiet = new SqlCommand(sqlChiTiet, conn, trans);
                        cmdChiTiet.Parameters.AddWithValue("@MaPhieu", maPhieuTao);
                        cmdChiTiet.Parameters.AddWithValue("@MaPhuTung", item.MaPhuTung);
                        cmdChiTiet.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                        cmdChiTiet.Parameters.AddWithValue("@TienCong", item.TienCong);

                        cmdChiTiet.ExecuteNonQuery();
                    }

                    trans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    errorMsg = "Lỗi hệ thống: " + ex.Message;
                    return false;
                }
            }
        }

        public decimal ThanhToan(int maPhieu)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlTinhTien = @"
                            SELECT ISNULL(SUM(ct.SoLuong * pt.DonGia + ct.TienCong), 0)
                            FROM ChiTietSuaChua ct
                            JOIN PhuTung pt ON ct.MaPhuTung = pt.MaPhuTung
                            WHERE ct.MaPhieu = @MaPhieu;";
                        SqlCommand cmdTinhTien = new SqlCommand(sqlTinhTien, conn, trans);
                        cmdTinhTien.Parameters.AddWithValue("@MaPhieu", maPhieu);

                        decimal tongTien = Convert.ToDecimal(cmdTinhTien.ExecuteScalar());

                        string sqlUpdatePhieu = @"
                            UPDATE PhieuSuaChua 
                            SET TongTien = @TongTien, 
                                TrangThai = N'Đã thanh toán' 
                            WHERE MaPhieu = @MaPhieu;";
                        SqlCommand cmdUpdate = new SqlCommand(sqlUpdatePhieu, conn, trans);
                        cmdUpdate.Parameters.AddWithValue("@TongTien", tongTien);
                        cmdUpdate.Parameters.AddWithValue("@MaPhieu", maPhieu);

                        cmdUpdate.ExecuteNonQuery();

                        trans.Commit();
                        return tongTien;
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
