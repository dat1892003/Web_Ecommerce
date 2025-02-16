using SV21T1020123.DataLayers;
using SV21T1020123.DataLayers.SQLServer;
using SV21T1020123.DomainModels;

namespace SV21T1020123.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;
        static UserAccountService() 
        {
            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(Configuration.ConnectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(Configuration.ConnectionString);
        }
        public static UserAccount? Authorize(UserTypes userTypes, string username, string password)
        {
            if (userTypes == UserTypes.Employee)
            {
                return employeeAccountDB.Authorzire(username, password);
            }
            else
            {
                return customerAccountDB.Authorzire(username,password);
            }
        }
        public static bool ChangePassword(UserTypes userTypes, string username, string password)
        {
            if (userTypes == UserTypes.Employee)
            {
                return employeeAccountDB.ChangePassword(username, password);
            }
            else
            {
                return customerAccountDB.ChangePassword(username, password);
            }    
        }
        public static int Singup(Customer data)
        {
            return customerAccountDB.SignUp(data);
        }
        public enum UserTypes
        {
            Employee,
            Customer,
        }
    }
}
