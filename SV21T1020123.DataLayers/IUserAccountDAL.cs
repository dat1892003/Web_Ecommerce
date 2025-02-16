using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers
{
    public interface IUserAccountDAL
    {
        UserAccount? Authorzire(string username, string password);
        bool ChangePassword(string username, string password);
        int SignUp(Customer data);
    }
}
