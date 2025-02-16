using SV21T1020123.DomainModels;
using SV21T1020123.DataLayers;

namespace SV21T1020123.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ISimpleQueryDAL<Province> provinceDB;
        private static readonly ISimpleQueryDAL<OrderStatus> orderstatusDB;

        /// <summary>
        /// Hàm khởi tạo 
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString); 
            shipperDB = new DataLayers.SQLServer.ShipperDAL(connectionString);
            categoryDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
            supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
            employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
            provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);
            orderstatusDB = new DataLayers.SQLServer.OrderStatusDAL(connectionString);
        }
        public static List<OrderStatus> ListofOrderStatus()
        {
            return orderstatusDB.List();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách thông tin tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListofProvinces()
        {
            return provinceDB.List();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu dưới dạng phân trang cho khách hàng
        /// </summary>
        /// <param name="rowCount">tham số đầu ra cho biết giá trị tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">Tên khách hàng hoặc tên giao dịch cần tìm</param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount, int page=1, int pageSize=0,string searchValue= "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue);
        }

        /// <summary>
        /// Lấy danh sách khách hàng 
        /// </summary>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers()
        {
            return customerDB.List();
        }
        /// <summary>
        /// thêm mới 1 khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        /// <summary>
        /// Trả về Id của khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        /// <summary>
        /// Xóa thông tin khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if (customerDB.InUsed(id))
                return false;
            return customerDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra khách hàng đang có đơn hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCustomer(int id)
        {
            return customerDB.InUsed(id);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu phân trang cho shipper
        /// </summary>
        /// <param name="rowCount">Than số đầu ra cho biết giá trị tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng hiển thị của mỗi trang</param>
        /// <param name="searchValue">Tên Shipper cần tìm</param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy danh sách tất cả Shipper
        /// </summary>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers()
        {
            return shipperDB.List();
        }
        /// <summary>
        /// Lấy thông tin shipper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id) { 
            return shipperDB.Get(id);
        }
        /// <summary>
        /// Thêm mới 1 shipper 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// Kiểm tra xem shipper có đơn hàng hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        /// <summary>
        /// Xóa thông tin shipper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id) {
            if (shipperDB.InUsed(id))
            {
                return false;
            }
            else
            {
                return shipperDB.Delete(id);
            }
        }
        /// <summary>
        /// Cập nhật thông tin shipper
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data) { 
            return shipperDB.Update(data);
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách dữ liệu phân trang cho loại hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy về danh sách tất cả loại hàng
        /// </summary>
        /// <returns></returns>
        public static List<Category> ListOfCategories()
        {
            return categoryDB.List();
        }
        /// <summary>
        /// Thêm loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// Lấy thông tin loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id) 
        { 
            return categoryDB.Get(id);
        }
        /// <summary>
        /// Cập nhật thông tin loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// Kiểm tra xem loại hàng có đang được sử dụng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCategory(int id) 
        { 
            return categoryDB.InUsed(id);
        }
        /// <summary>
        /// Xóa loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id) 
        {
            if (InUsedCategory(id))
            {
                return false;
            }
            else 
            { 
                return categoryDB.Delete(id);
            }
        }
        /// <summary>
        /// Tìm kiếm và lấy dữ liệu phân trang cho nhà cung cấp
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pagSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pagSize = 0, string searchValue = "") 
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pagSize, searchValue);
        }
        /// <summary>
        /// Thêm mới một nhà cung cấp
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers()
        {
            return supplierDB.List();
        }
        public static int AddSupplier(Supplier supplier)
        {
            return supplierDB.Add(supplier);
        }
        /// <summary>
        /// Kiểm tra nhà cung cấp đang có sản phẩm hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedSupllier(int id)
        {
            return supplierDB.InUsed(id);
        }
        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (InUsedSupllier(id))
            {
                return false;
            }
            else
            {
                return supplierDB.Delete(id);
            }    
        }
        /// <summary>
        /// Lấy thông tin nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id) {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Cập nhật thông tin nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// Tìm kiếm và lấy dữ liệu phân trang cho nhân viên
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page=1, int pageSize =0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue);
        }
        /// <summary>
        /// Lấy danh sách nhân viên
        /// </summary>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees()
        {
            return employeeDB.List();
        }

        /// <summary>
        /// Kiểm tra xem nhân viên đang có đơn hàng hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InusedEmployee(int id) {
            return employeeDB.InUsed(id);
        }
        /// <summary>
        /// Xóa thông tin nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id) {
            if (InusedEmployee(id))
            {
                return false;
            }
            else
            {
                return employeeDB.Delete(id);
            }
        }
        /// <summary>
        ///Thêm mới một nhân viên
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        /// <summary>
        /// Lấy thông tin nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id) { 
            return employeeDB.Get(id);
        }
        /// <summary>
        /// Cập nhật thông tin nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
           return employeeDB.Update(data);
        }
    }
}