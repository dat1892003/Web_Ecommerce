using Dapper;
using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }
        UserAccount? IUserAccountDAL.Authorzire(string username, string password)
        {
            UserAccount? data = null;
            using(var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserID,
	                       Email as UserName,
	                       FullName as DisplayName, 
	                       Photo,
	                       Role
                    from Employees
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
                var sql = @"UPDATE Employees 
                        set Password = @password
                        where Email = @username";
                var parameters = new
                {
                    password,
                    username
                };
                result = connection.Execute(sql,parameters,commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
        public int SignUp(Customer data)
        {
            throw new NotImplementedException();
        }
    }
}
