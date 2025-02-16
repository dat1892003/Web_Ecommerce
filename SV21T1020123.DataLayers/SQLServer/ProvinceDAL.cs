using Dapper;
using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class ProvinceDAL : BaseDAL, ISimpleQueryDAL<Province>
    {
        public ProvinceDAL(string connectionString) : base(connectionString)
        {
        }

        public List<Province> List()
        {
            List<Province> data = new List<Province>();
            using (var connection = OpenConnection())
            {
                var sql = @"Select * from Provinces";
                data = connection.Query<Province>(sql, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
    }
}
