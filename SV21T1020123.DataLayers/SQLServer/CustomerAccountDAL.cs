using Dapper;
using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public int SignUp(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Customers where Email = @Email)
                            select -1
                            else
                            begin
                                insert into Customers(CustomerName,ContactName,Province,Address,Phone,Email,Islocked,Password,Role)
                                values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @Islocked,@Password,@Role);
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
                    Password = data.Password ?? "",
                    IsLocked = data.IsLocked,
                    Role = "user"
                };
                id = connection.ExecuteScalar<int>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        UserAccount? IUserAccountDAL.Authorzire(string username, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select CustomerID as UserID,
	                               Email as UserName,
	                               CustomerName as DisplayName, 
	                               Role
                            from Customers
                            where Email = @UserName and Password = @Password";
                var parameters = new
                {
                    UserName = username,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql, parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        bool IUserAccountDAL.ChangePassword(string username, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"UPDATE Customers 
                        set Password = @password
                        where Email = @username";
                var parameters = new
                {
                    password,
                    username
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
