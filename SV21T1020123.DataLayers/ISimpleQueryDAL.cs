namespace SV21T1020123.DataLayers
{
    /// <summary>
    /// Tạo interface có hàm truy vấn dữ liệu
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISimpleQueryDAL<T> where T : class
    {
        /// <summary>
        /// Truy vấn và lấy toàn bộ dữ liệu của bảng
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
