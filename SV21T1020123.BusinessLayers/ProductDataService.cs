using SV21T1020123.DataLayers;
using SV21T1020123.DataLayers.SQLServer;
using SV21T1020123.DomainModels;
using System.ComponentModel.DataAnnotations;

namespace SV21T1020123.BusinessLayers
{
    public class ProductDataService
    {
        private static readonly IProductDAL productDB;
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }
        /// <summary>
        /// Tìm kiếm danh sách mặt hàng không phân trang
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(string searchValue = "")
        {
            return productDB.List(searchValue: searchValue);
        }
        /// <summary>
        /// Tìm kiếm và lất danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowcount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(out int rowcount,int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID =0, 
                                                   decimal minPrice = 0, decimal maxPrice =0)
        {
            rowcount = productDB.Count(searchValue,categoryID,supplierID,minPrice,maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice);
        }
        /// <summary>
        /// Lấy thông tin mặt hàng theo mã hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        /// <summary>
        /// Thêm mới một mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin một mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data) 
        { 
            return productDB.Update(data);
        }
        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID)
        {
            return productDB.Delete(productID);
        }
        /// <summary>
        /// Kiểm mặt hàng có đơn hàng liên quan hay không
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }
        /// <summary>
        /// Lấy danh sách của mặt hàng theo thứ tự DisplayOrder
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return (List<ProductPhoto>)productDB.ListPhoto(productID);
        }
        /// <summary>
        /// Lấy thông tin 1 ảnh dựa vào ID
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static ProductPhoto? GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }
        /// <summary>
        /// Bổ sung ảnh cho mặt hàng (trả về mã ảnh được bổ sung)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }
        /// <summary>
        /// Cập nhật ảnh của măt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }
        /// <summary>
        /// Xóa ảnh của mặt hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }
        /// <summary>
        /// Lấy danh sách thuộc tính hàng hóa
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return (List<ProductAttribute>)productDB.ListAttribute(productID);
        }
        /// <summary>
        /// Lấy 1 thuộc tính hàng hóa
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }
        /// <summary>
        /// Thêm thuộc tính hàng hóa
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }
        /// <summary>
        /// Cập nhật thuộc tính hàng hóa
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }
        /// <summary>
        /// Xóa thuộc tính hàng hóa
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }
    }
}
