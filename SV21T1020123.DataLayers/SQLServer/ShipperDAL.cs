using Dapper;
using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class ShipperDAL : BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Shippers where Phone = @Phone)
                            select -1
                            else
                            begin
                            insert into Shippers(ShipperName, Phone)
                            values (@ShipperName, @Phone);
                            select scope_identity();
                            end";
                var parameters = new
                {
                    ShipperName = data.ShipperName,
                    Phone = data.Phone,
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
                var sql = @"select COUNT(*)
                        	from Shippers
                            where (ShipperName like @searchValue)";
                var parameters = new
                {
                    searchValue,
                };
                count = connection.ExecuteScalar<int>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            var result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Shippers where ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = id,
                };
                result = connection.Execute(sql,parameters,commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? data = null;
            using (var connection = OpenConnection()) 
            {
                var sql = @"select * from Shippers where ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = id,
                };
                data = connection.QueryFirstOrDefault<Shipper>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            };
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Orders where ShipperID = @ShipperID";
                var parameter = new
                {
                    shipperID = id,
                };
                result = connection.ExecuteScalar<bool>(sql, parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> data = new List<Shipper>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from(
	                            select * , ROW_NUMBER() over(order by ShipperName ) as RowNumber
	                            from Shippers where (ShipperName like @searchValue)
	                            ) as t
                            where (@pageSize = 0 ) or ( t.RowNumber between (@page-1)* @pageSize + 1 and (@page*@pageSize) )";
                var parameters = new
                {
                    page,
                    pageSize,
                    searchValue,
                };
                data = connection.Query<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
        public List<Shipper> List()
        {
            List<Shipper> data = new List<Shipper>();

            using (var connection = OpenConnection())
            {
                var sql = @"select * from Shippers";
                data = connection.Query<Shipper>(sql: sql, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = OpenConnection()) 
            {
                var sql = @"if not exists(select * from Shippers where ShipperID <> @ShipperID and Phone = @Phone)
                            begin
                            update Shippers 
                            Set ShipperName = @ShipperName,
	                            Phone = @Phone
                            where ShipperID = @ShipperID
                            end";
                var parameter = new
                {
                    ShipperID = data.ShipperID,
                    ShipperName = data.ShipperName,
                    Phone = data.Phone,
                };
                result = connection.Execute(sql,parameter,commandType: System.Data.CommandType.Text) > 0; 
                connection.Close();
            }
            return result;
        }
    }
}
