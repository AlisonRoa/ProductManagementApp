using ProductManagementApp.DTO;
using ProductManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ProductManagementApp.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly string _cs;

        public ProductsRepository()
        {
            _cs = ConfigurationManager.ConnectionStrings["ProductManagement"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(_cs))
                _cs = Environment.GetEnvironmentVariable("PM_CONNSTR");

            if (string.IsNullOrWhiteSpace(_cs))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'ProductManagement'. " +
                    "Agrega <connectionStrings> en App.config del proyecto WPF o define la variable " +
                    "de entorno PM_CONNSTR."
                );
            }
        }

        private SqlConnection CreateConnection() => new SqlConnection(_cs);

        public async Task<List<ProductListItem>> GetProductsAsync()
        {
            const string sql = @"
                SELECT 
                    p.Id,
                    p.ProductCode,
                    p.ProductName,
                    sc.StatusName,
                    s.SupplierName,
                    ISNULL(st.InStock, 0) AS InStock,
                    p.PricePerUnit,
                    p.BasicUnit,
                    ISNULL(stu.LastUpdate, SYSDATETIME()) AS CreatedDate
                FROM PM.Products p
                JOIN PM.StatusCatalog sc ON sc.Id = p.StatusId
                JOIN PM.Suppliers s       ON s.Id  = p.SuppliersId
                LEFT JOIN (
                    SELECT ProductsId, SUM(InStock) AS InStock
                    FROM PM.Stocks
                    GROUP BY ProductsId
                ) st  ON st.ProductsId  = p.Id
                LEFT JOIN (
                    SELECT ProductsId, MAX(LastUpdate) AS LastUpdate
                    FROM PM.Stocks
                    GROUP BY ProductsId
                ) stu ON stu.ProductsId = p.Id
                ORDER BY p.ProductName ASC;";

            var list = new List<ProductListItem>();

            using (var cn = CreateConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                await cn.OpenAsync().ConfigureAwait(false);
                using (var rd = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false))
                {
                    int iId = rd.GetOrdinal("Id");
                    int iCode = rd.GetOrdinal("ProductCode");
                    int iName = rd.GetOrdinal("ProductName");
                    int iStatus = rd.GetOrdinal("StatusName");
                    int iSupplier = rd.GetOrdinal("SupplierName");
                    int iInStock = rd.GetOrdinal("InStock");
                    int iPrice = rd.GetOrdinal("PricePerUnit");
                    int iBasicUnit = rd.GetOrdinal("BasicUnit");
                    int iCreatedDate = rd.GetOrdinal("CreatedDate");

                    while (await rd.ReadAsync().ConfigureAwait(false))
                    {
                        list.Add(new ProductListItem
                        {
                            Id = rd.GetInt32(iId),
                            ProductCode = rd.GetString(iCode),
                            ProductName = rd.GetString(iName),
                            StatusName = rd.GetString(iStatus),
                            SupplierName = rd.GetString(iSupplier),
                            InStock = rd.IsDBNull(iInStock) ? 0m : Convert.ToDecimal(rd.GetValue(iInStock)),
                            PricePerUnit = rd.GetDecimal(iPrice),
                            BasicUnit = rd.GetString(iBasicUnit),
                            CreatedDate = rd.IsDBNull(iCreatedDate) ? DateTime.Now : Convert.ToDateTime(rd.GetValue(iCreatedDate))
                        });
                    }
                }
            }
            return list;
        }

        public async Task<List<StatusItem>> GetStatusesAsync()
        {
            const string sql = @"SELECT Id, StatusName FROM PM.StatusCatalog ORDER BY StatusName;";
            var list = new List<StatusItem> { new StatusItem { Id = 0, Name = "Todos" } };

            using (var cn = CreateConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                await cn.OpenAsync().ConfigureAwait(false);
                using (var rd = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false))
                {
                    int iId = rd.GetOrdinal("Id");
                    int iName = rd.GetOrdinal("StatusName");

                    while (await rd.ReadAsync().ConfigureAwait(false))
                    {
                        list.Add(new StatusItem
                        {
                            Id = rd.GetInt32(iId),
                            Name = rd.GetString(iName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IList<OptionItem>> GetOptionsByProductAsync(int productId)
        {
            var list = new List<OptionItem>();
            const string sql = @"
                SELECT o.Id, o.OptionCode, o.OptionName, o.StatusId, s.StatusName, o.ProductsId AS ProductId
                FROM PM.Options o
                JOIN PM.StatusCatalog s ON s.Id = o.StatusId
                WHERE o.ProductsId = @pid
                ORDER BY o.OptionName;";

            using (var cn = CreateConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@pid", SqlDbType.Int).Value = productId;
                await cn.OpenAsync().ConfigureAwait(false);
                using (var rd = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection).ConfigureAwait(false))
                {
                    while (await rd.ReadAsync().ConfigureAwait(false))
                    {
                        list.Add(new OptionItem
                        {
                            Id = rd.GetInt32(0),
                            OptionCode = rd.GetString(1),
                            OptionName = rd.GetString(2),
                            StatusId = rd.GetInt32(3),
                            StatusName = rd.GetString(4),
                            ProductsId = rd.GetInt32(5)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<OptionItem> SaveOptionAsync(OptionItem o)
        {
            using (var cn = CreateConnection())
            {
                await cn.OpenAsync().ConfigureAwait(false);

                if (o.Id == 0)
                {
                    const string ins = @"
                        INSERT INTO PM.Options (OptionCode, OptionName, StatusId, ProductsId)
                        VALUES (@code, @name, @sid, @pid);
                        SELECT SCOPE_IDENTITY();";
                    using (var cmd = new SqlCommand(ins, cn))
                    {
                        cmd.Parameters.AddWithValue("@code", o.OptionCode);
                        cmd.Parameters.AddWithValue("@name", o.OptionName);
                        cmd.Parameters.AddWithValue("@sid", o.StatusId);
                        cmd.Parameters.AddWithValue("@pid", o.ProductsId);
                        var idObj = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                        o.Id = Convert.ToInt32(idObj);
                    }
                }
                else
                {
                    const string upd = @"
                        UPDATE PM.Options
                        SET OptionCode=@code, OptionName=@name, StatusId=@sid
                        WHERE Id=@id;";
                    using (var cmd = new SqlCommand(upd, cn))
                    {
                        cmd.Parameters.AddWithValue("@code", o.OptionCode);
                        cmd.Parameters.AddWithValue("@name", o.OptionName);
                        cmd.Parameters.AddWithValue("@sid", o.StatusId);
                        cmd.Parameters.AddWithValue("@id", o.Id);
                        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }
                }
            }
            return o;
        }

        public async Task DeleteOptionAsync(int optionId)
        {
            const string sql = @"DELETE FROM PM.Options WHERE Id = @Id;";

            using (var cn = CreateConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@Id", optionId);
                await cn.OpenAsync().ConfigureAwait(false);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task<bool> OptionCodeExistsAsync(string code, int? excludeId = null)
        {
            const string sql = @"SELECT CASE WHEN EXISTS(
                                        SELECT 1 FROM PM.Options 
                                        WHERE OptionCode = @code AND (@ex IS NULL OR Id <> @ex)
                                    ) THEN 1 ELSE 0 END;";
            using (var cn = CreateConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@code", code ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ex", (object)excludeId ?? DBNull.Value);
                await cn.OpenAsync().ConfigureAwait(false);
                var val = (int)await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                return val == 1;
            }
        }

        public async Task<string> GetNextOtpCodeAsync()
        {
            const string sql = @"SELECT 'OTP-' + RIGHT('000' + CAST(ISNULL(MAX(CAST(SUBSTRING(OptionCode,5,10) AS INT)),0) + 1 AS VARCHAR(10)), 3)
                                FROM PM.Options
                                WHERE OptionCode LIKE 'OTP-%';";
            using (var cn = CreateConnection())
            using (var cmd = new SqlCommand(sql, cn))
            {
                await cn.OpenAsync().ConfigureAwait(false);
                var s = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
                return Convert.ToString(s);
            }
        }


    }
}
