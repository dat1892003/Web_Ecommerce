namespace SV21T1020123.BusinessLayers
{
    public class Configuration
    {   
        private static string connectionString = "";
        /// <summary>
        /// Tạo cấu hình cho BusinessLayer
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.connectionString = connectionString;
        }
        /// <summary>
        /// Chuỗi tham số kết nối đến CSDL
        /// </summary>
        public static string ConnectionString 
        {
            get 
            {
                return connectionString; 
            } 
        }
    }
}
