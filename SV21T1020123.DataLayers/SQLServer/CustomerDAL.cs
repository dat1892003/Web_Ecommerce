using SV21T1020123.DomainModels;
using Dapper;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class CustomerDAL : BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }
        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Customers where Email = @Email)
                            select -1
                            else
                            begin
                                insert into Customers(CustomerName,ContactName,Province,Address,Phone,Email,Islocked)
                                values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Islocked);
                                select scope_identity();
                            end";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked,
                };
                id = connection.ExecuteScalar<int>(sql, parameters, commandType: System.Data.CommandType.Text);    
                connection.Close();
            }    
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"Select COUNT(*) 
                            from Customers
                            where (CustomerName  like @searchValue) or (ContactName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue,
                };
                count=connection.ExecuteScalar<int>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"Delete from Customers where CustomerID=@CustomerID";
                var parameter = new
                {
                    CustomerID= id,
                };
                result = connection.Execute(sql,parameter,commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"Select * from Customers where CustomerID=@CustomerID";
                var parameters = new
                {
                    CustomerID = id,
                };
                data = connection.QueryFirstOrDefault<Customer>(sql,parameters,commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using(var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Orders where CustomerID=@CustomerID)
                            select 1
                            else
                            select 0";
                var parameter = new
                {
                    CustomerId = id,
                };
                result = connection.ExecuteScalar<bool>(sql, parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from(
                                    Select * , row_number() over(order by CustomerName) as RowNumber
                                    from Customers
                                       where (CustomerName  like @searchValue) or (ContactName like @searchValue)
                                ) as t
                            where (@pageSize=0) or (t.RowNumber between (@page - 1) *  @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,   //bên trái: tên tham số câu lệnh SQL// bên phải giá trị truyền vào
                    pageSize = pageSize,
                    searchValue = searchValue,  
                };
                data = connection.Query<Customer>(sql: sql,param: parameters, commandType:System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
        public List<Customer> List()
        {
            List<Customer> data = new List<Customer>();
            using (var connection = OpenConnection())
            {
                var sql = @"Select * from Customers";
                data = connection.Query<Customer>(sql, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Customers where CustomerID <> @CustomerID and Email = @email)
                            begin
                                Update Customers 
                                set CustomerName=@CustomerName, 
                                ContactName=@ContactName,  
                                Province=@Province,
                                Address=@Address,
                                Phone=@Phone,
                                Email=@Email,
                                IsLocked=@IsLocked
                                where CustomerID = @CustomerID;
                            end";
                var parameter = new
                {   
                    CustomerID= data.CustomerID,
                    CustomerName= data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked,
                };
                result = connection.Execute(sql, parameter, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
