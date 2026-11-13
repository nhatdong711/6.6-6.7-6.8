using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using BilliardsManagement.DTO;

namespace BilliardsManagement.DAL
{
    public class BanBidaDAL
    {
        public List<BanBidaDTO> GetAll()
        {
            var list = new List<BanBidaDTO>();
            using (var conn = DBConnect.GetConnection())
            using (var cmd = new SqlCommand("SELECT MaBan, TenBan, LoaiBan, GiaTheoGio, TrangThai FROM BanBida ORDER BY MaBan", conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                    while (rd.Read())
                        list.Add(new BanBidaDTO
                        {
                            MaBan = rd.GetInt32(0), TenBan = rd.GetString(1), LoaiBan = rd.IsDBNull(2) ? "" : rd.GetString(2),
                            GiaTheoGio = rd.GetDecimal(3), TrangThai = rd.GetString(4)
                        });
            }
            return list;
        }

        public int MoBan(int maBan)
        {
            using (var conn = DBConnect.GetConnection())
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();
                try
                {
                    var upd = new SqlCommand("UPDATE BanBida SET TrangThai = @dc WHERE MaBan = @b AND TrangThai = @tr", conn, trans);
                    upd.Parameters.AddWithValue("@dc", "Đang chơi");
                    upd.Parameters.AddWithValue("@tr", "Trống");
                    upd.Parameters.AddWithValue("@b", maBan);
                    if (upd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Bàn không ở trạng thái Trống.");
                    var ins = new SqlCommand(
                        "INSERT INTO HoaDon(MaBan, GioBatDau, TrangThai) VALUES(@b, GETDATE(), 0); SELECT CAST(SCOPE_IDENTITY() AS int);", conn, trans);
                    ins.Parameters.AddWithValue("@b", maBan);
                    int maHD = Convert.ToInt32(ins.ExecuteScalar());
                    trans.Commit();
                    return maHD;
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
