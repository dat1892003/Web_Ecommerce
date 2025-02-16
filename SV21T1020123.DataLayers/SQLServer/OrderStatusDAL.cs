using Dapper;
using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class OrderStatusDAL : BaseDAL,ISimpleQueryDAL<OrderStatus>
    {
        public OrderStatusDAL(string connectionString) : base(connectionString)
        {
        }

        public List<OrderStatus> List()
        {
            var data = new List<OrderStatus>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from OrderStatus";
                data = connection.Query<OrderStatus>(sql, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
    }
}
