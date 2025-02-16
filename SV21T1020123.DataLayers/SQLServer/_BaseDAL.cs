using Microsoft.Data.SqlClient;

namespace SV21T1020123.DataLayers.SQLServer
{
    /// <summary>
    /// Lớp cơ sở (lớp cha) của các lớp cài đặt sử lý dữ liệu trên SQL Server
    /// </summary>
    public abstract class BaseDAL
    {
        /// <summary>
        /// Chuỗi tham số kết nối đến cơ sở dữ liệu
        /// </summary>
        protected string connectionString = "";
        /// <summary>
        /// Hàm khỏi tạo chuỗi kết nối
        /// </summary>
        /// <param name="connectionString"></param>
        public BaseDAL(string connectionString) {
            this.connectionString = connectionString;
        }
        /// <summary>
        /// Tạo và mở một kết nối đến CSDL (SQL Server)
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
