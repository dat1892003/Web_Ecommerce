using Dapper;
using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class EmployeeDAL : BaseDAL,ICommonDAL<Employee>
    {
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Employee data)
        {
            int id = 0;
            using(var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Employees where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName, BirthDate, Address, Phone, Email, Photo, IsWorking)
                                    values(@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    FullName =data.FullName ?? "",
                    Address = data.Address ?? "",
                    BirthDate = data.BirthDate,
                    Email = data.Email ?? "",
                    Phone = data.Phone ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking,
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
                var sql = @"select Count(*)
                            from Employees
                            where FullName like @searchValue";
                var parameter = new
                {
                    searchValue,
                };
                count = connection.ExecuteScalar<int>(sql, parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Employees
                            where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id,
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Employee? Get(int id)
        {
            Employee? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Employees
                            where EmployeeID = @EmployeeID";
                var parameters = new
                {
                    EmployeeID = id,
                };
                data = connection.QueryFirstOrDefault<Employee>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Orders where EmployeeID = @EmployeeID)
                            select 1
                            else 
                            select 0";
                var parameter = new
                {
                    EmployeeID = id,
                };
                result = connection.ExecuteScalar<bool>(sql, parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Employee> data = new List<Employee>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select * 
                            from (
                            select *, ROW_NUMBER() over (order by FullName) as RowNumber
                            from Employees
                            where FullName like @searchValue
                            ) as t
                            where (@pageSize=0) or (t.RowNumber between (@page-1)*@pageSize+1 and(@page*@pageSize))";
                var parameter = new
                {
                    page,
                    pageSize,
                    searchValue,
                };
                data = connection.Query<Employee>(sql,parameter,commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
        public List<Employee> List()
        {
            List<Employee> data = new List<Employee>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Employees";
                data = connection.Query<Employee>(sql,commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Employee data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Employees where EmployeeID <> @EmployeeID and Email = @Email)
                                begin
                                    update Employees 
                                    set FullName = @FullName,
                                        BirthDate = @BirthDate,
                                        Address = @Address,
                                        Phone = @Phone,
                                        Email = @Email,
                                        Photo = @Photo,
                                        IsWorking = @IsWorking
                                        where EmployeeID = @EmployeeID;
                               end";
                var parameter = new
                {
                    FullName = data.FullName ?? "",
                    Address = data.Address ?? "",
                    BirthDate = data.BirthDate,
                    Email = data.Email ?? "",
                    Phone = data.Phone ?? "",
                    Photo = data.Photo ?? "",
                    IsWorking = data.IsWorking,
                    EmployeeID = data.EmployeeID,
                };
                result = connection.Execute(sql, parameter, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
