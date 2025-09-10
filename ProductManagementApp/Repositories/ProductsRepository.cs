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
    }
}
