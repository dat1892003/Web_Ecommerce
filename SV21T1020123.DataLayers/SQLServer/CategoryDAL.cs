using Dapper;
using SV21T1020123.DomainModels;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class CategoryDAL : BaseDAL,ICommonDAL<Category>,ISimpleQueryDAL<Category>
    {
        public CategoryDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Category data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Categories where CategoryName = @CategoryName)
                            select -1
                            else
                            begin
                                insert into Categories(CategoryName, Description)
                                values(@CategoryName, @Description);
                                select scope_identity();
                            end";
                var parameters = new
                {
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",
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
                            from Categories
                            where CategoryName like @searchValue";
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
                var sql = @"Delete 
                            from Categories 
                            where CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryID = id,
                };
                result = connection.Execute(sql, parameters,commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Category? Get(int id)
        {   Category? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Categories where CategoryID = @CategoryID";
                var parameters = new
                {
                    CategoryID = id, 
                };
                data = connection.QueryFirstOrDefault<Category>(sql, parameters,commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection()) 
            {
                var sql = @"if exists(select * from Products where CategoryID=@CategoryID)
                            select 1
                            else 
                            select 0";
                var parameters = new
                {
                    CategoryID= id,
                };
                result = connection.ExecuteScalar<bool>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
           var data = new List<Category>();
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select *
                            from(
                            select *, row_number() over (order by CategoryName) as RowNumber
                            from Categories
                            where CategoryName like @searchValue) as t
                            where @pageSize=0 or (t.RowNumber between (@page-1)*@pageSize+1 and @page*@pageSize)";
                var parameters = new
                {
                    page,
                    pageSize,
                    searchValue,
                };
                data = connection.Query<Category>(sql,parameters,commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public List<Category> List()
        {
            var data = new List<Category>();
            using(var connection = OpenConnection())
            {
                var sql = @"select * from Categories";
                connection.Close();
                data = connection.Query<Category>(sql, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            };  
            return data;
        }

        public bool Update(Category data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Categories where CategoryID <> @CategoryID and CategoryName = @CategoryName)
                            begin
                                Update Categories
                                set CategoryName = @CategoryName,
	                                Description = @Description
                                where CategoryID = @CategoryID;
                            end";
                var parameters = new
                {
                    CategoryID = data.CategoryID,
                    CategoryName = data.CategoryName ?? "",
                    Description = data.Description ?? "",
                };
                result = connection.Execute(sql,parameters,commandType:System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
