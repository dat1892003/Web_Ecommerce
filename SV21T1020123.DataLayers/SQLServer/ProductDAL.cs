using Dapper;
using SV21T1020123.DomainModels;
using System;

namespace SV21T1020123.DataLayers.SQLServer
{
    public class ProductDAL : BaseDAL,IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from Products where ProductName = @ProductName)
                            select -1
                            else
                                begin
                                    insert into Products(ProductName, ProductDescription, CategoryID, SupplierID, Unit, Price, Photo, IsSelling)
                                    values (@ProductName, @ProductDescription, @CategoryID, @SupplierID, @Unit, @Price, @Photo, @IsSelling);
                                    select @@IDENTITY
                                end";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling,
                };
                id = connection.ExecuteScalar<int>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            long id =  0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from ProductAttributes where ProductID = @ProductID and AttributeName = @AttributeName)
                            select -1
                            else if exists (select * from ProductAttributes where ProductID = @ProductID and  DisplayOrder = @DisplayOrder)
                            select -2
                            else
                                begin
                                    insert into ProductAttributes(AttributeName, AttributeValue, DisplayOrder, ProductID)
                                    values (@AttributeName, @AttributeValue, @DisplayOrder, @ProductID);
                                    select @@IDENTITY;
                                 end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                };
                id = connection.ExecuteScalar<long>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            long id = 0;
            using (var connection = OpenConnection()) 
            {
                var sql = @"if exists (select * from ProductPhotos where ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                            select -1
                            else
                                begin
                            insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                            values (@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                            select @@IDENTITY;
                                 end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                };
                id = connection.ExecuteScalar<long>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Products
	                                where (@SearchValue = N'' or ProductName like @SearchValue)
		                                AND (@CategoryID = 0 or @CategoryID=CategoryID)
		                                AND (@SupplierID = 0 or SupplierID =@SupplierID)
		                                AND (Price >= @MinPrice)
		                                AND ( @MaxPrice<=0 or Price <= @MaxPrice) ";
                var parameters = new
                {
                    SearchValue = searchValue ?? "",
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };
                count = connection.ExecuteScalar<int>(sql,parameters,commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete ProductAttributes where ProductID = @ProductID;
                            delete ProductPhotos where ProductID = @ProductID
                            delete Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID,
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
                return result;
        }

        public bool DeleteAttribute(long AttributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete ProductAttributes
                            where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = AttributeID,
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete ProductPhotos
                           where PhotoID = @PhotoID";
                var parameters = new
                {
                    photoID = photoID
                };
                result = connection.Execute(sql,parameters,commandType:System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int productID)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products
                            where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = productID,
                };
                data = connection.QueryFirstOrDefault<Product>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes 
                            where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long productID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos
                            where ProductID = @ProductID ";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists (select * from OrderDetails where ProductID = @ProductID) 
                            select 1
                            else 
                            select 0";
                var parameters = new
                {
                    ProductID = productID
                };
                result = connection.ExecuteScalar<bool>(sql, parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var data = new List<Product>();
            searchValue = $"%{searchValue}%";
            using(var connection = OpenConnection())
            {
                string sql = @"select *
                                from(select *, ROW_NUMBER() over (order by productName) as RowNumber from Products
	                                where (@SearchValue = N'' or ProductName like @SearchValue)
		                                AND (@CategoryID = 0 or CategoryID = @CategoryID)
		                                AND (@SupplierID = 0 or SupplierID = @SupplierID)
		                                AND (Price >= @MinPrice)
		                                AND ( @MaxPrice<=0 or Price <= @MaxPrice )) as t
                                    where (@PageSize = 0) or (t.RowNumber between (@Page-1) * @PageSize + 1 and (@Page*@PageSize))";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                    CategoryID = categoryID,
                    SupplierID = supplierID,
                    Page = page,
                    PageSize = pageSize,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                };
                data = connection.Query<Product>(sql,parameters,commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
                return data;
        }

        public IList<ProductAttribute> ListAttribute(int productID)
        {
            var data = new List<ProductAttribute>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes
                            where ProductID = @ProductID
                            order by DisplayOrder asc";
                var parameters = new
                {
                    ProductID = productID
                };
                data = connection.Query<ProductAttribute>(sql,parameters,commandType:System.Data.CommandType.Text).ToList() ;
                connection.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhoto(int productID)
        {
            var data = new List<ProductPhoto>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos
                            where ProductID = @ProductID
                            order by DisplayOrder asc";
                var parameters = new
                {
                    ProductID = productID 
                };
                data = connection.Query<ProductPhoto>(sql,parameters,commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                            begin
                                update Products 
                                set ProductName = @ProductName, 
	                                ProductDescription = @ProductDescription, 
	                                CategoryID = @CategoryID, 
                                    SupplierID = @SupplierID,
	                                Unit = @Unit, 
	                                Price = @Price, 
	                                Photo = @Photo, 
	                                IsSelling = @IsSelling
                                where ProductID = @ProductID;
                             end";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductID = data.ProductID,
                    ProductDescription = data.ProductDescription ?? "",
                    CategoryID = data.CategoryID,
                    SupplierID = data.SupplierID,
                    Unit = data.Unit ?? "" ,
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling,
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from ProductAttributes where AttributeID<>@AttributeID and ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                            begin
                            update ProductAttributes
                            set AttributeName = @AttributeName, 
	                            AttributeValue = @AttributeValue, 
	                            DisplayOrder = @DisplayOrder, 
	                            ProductID = @ProductID
                            where AttributeID = @AttributeID;
                            end";
                var parameters = new
                {
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder,
                    ProductID = data.ProductID,
                    AttributeID = data.AttributeID
                };
                result = connection.Execute(sql, parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from ProductPhotos where PhotoID <> @PhotoID and ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                            begin
                                update ProductPhotos 
                                set ProductID = @ProductID, 
	                                Photo =@Photo, 
	                                Description = @Description, 
	                                DisplayOrder = @DisplayOrder, 
	                                IsHidden = @IsHidden
                                where PhotoID = @PhotoID;
                              end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                    PhotoID = data.PhotoID
                };
                result = connection.Execute(sql,parameters,commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
