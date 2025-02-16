using Dapper;
using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class SupplierDAL : BaseDAL, ICommonDAL<Supplier>, ISimpleQueryDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connnection = OpenConnection())
            {
                var sql = @"if exists (select * from Suppliers where Email = @Email)
                            select -1
                            else
                            begin
                            insert into Suppliers(SupplierName, ContactName, Address, Phone, Province, Email)
                            values(@SupplierName, @ContactName, @Address, @Phone, @Province, @Email);
                            select scope_identity();
                            end";
                var parameters = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName= data.ContactName ?? "",
                    Address = data.Address?? "",
                    Phone = data.Phone ?? "",
                    Province = data.Province ?? "",
                    Email = data.Email ?? "",
                };
                id = connnection.ExecuteScalar<int>(sql, parameters, commandType: System.Data.CommandType.Text);
                connnection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using(var connection = OpenConnection()) 
            {
                var sql = @"select count(*)
                            from Suppliers
                            where (SupplierName like @searchValue) or (ContactName like @searchValue)";
                var parameters = new 
                {
                    searchValue,
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
                var sql = @"delete from Suppliers
                            where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id,
                };
                result = connection.Execute(sql,parameters,commandType: System.Data.CommandType.Text) > 0;
                connection.Close ();
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Suppliers where SupplierID = @SupplierID";
                var parameter = new
                {
                    SupplierId = id,
                };
                data = connection.QueryFirstOrDefault<Supplier>(sql, parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Products where SupplierID = @SupplierID)
                            select 1
                            else 
                            select 0";
                var parameters = new
                {
                    SupplierID = id,
                };
                result = connection.ExecuteScalar<bool>(sql, parameters, commandType:System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                            from(
	                            select *, ROW_NUMBER() over(order by SupplierName) as RowNumber
	                            from Suppliers
	                            where SupplierName like @searchValue or ContactName like @searchValue
                            ) as t
                            where (@pageSize=0) or (t.RowNumber between (@page-1)*@pageSize+1 and (@page*@pageSize))";
                var parameters = new
                {
                    page,
                    pageSize,
                    searchValue,
                };
                data = connection.Query<Supplier>(sql,parameters,commandType: System.Data.CommandType.Text).ToList();   
                connection.Close();
            }
            return data;
        }

        public List<Supplier> List()
        {
            var data = new List<Supplier>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Suppliers";
                data = connection.Query<Supplier>(sql,commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Suppliers where SupplierID <> @SupplierID and Email = @email)
                            begin
                            update Suppliers
                            set SupplierName = @SupplierName,
	                            ContactName = @ContactName, 
	                            Address= @Address,
	                            Phone = @Phone,
	                            Province = @Province,
	                            Email = @Email
                            where SupplierID = @SupplierID;
                            end";
                var parameters = new
                {
                    SupplierID = data.SupplierID,
                    SupplierName = data.SupplierName,
                    ContactName = data.ContactName,
                    Address = data.Address,
                    Phone = data.Phone,
                    Province = data.Province,
                    Email = data.Email,
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
