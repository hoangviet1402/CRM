using System.Net;
using System.Data;
using Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using Shared.Extensions;
using Company.Entities;

namespace Company.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly ApplicationDbContext _context;

    public CompanyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateCompanyAsync(string fullName, string address)
    {
        var connection = _context.Database.GetDbConnection();
        var companyId = 0;

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Company_Create";
            command.CommandType = System.Data.CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.Add(new SqlParameter("@FullName", System.Data.SqlDbType.NVarChar, 100) { Value = fullName });
            command.Parameters.Add(new SqlParameter("@Addess", System.Data.SqlDbType.NVarChar, 250) { Value = address });
            command.Parameters.Add(new SqlParameter("@CompanyId", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output });

            // Execute
            await command.ExecuteNonQueryAsync();
            companyId = Convert.ToInt32(command.Parameters["@CompanyId"].Value);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("CreateCompany Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == System.Data.ConnectionState.Open)
                await connection.CloseAsync();
        }

        return companyId;
    }

    public async Task<IEnumerable<EmployeeCompany>> GetEmployeeCompanyAsync(int employeeId)
    {
        var connection = _context.Database.GetDbConnection();
        var result = new List<EmployeeCompany>();

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = "Ins_Company_GetEmployeeID";
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters
            command.Parameters.Add(new SqlParameter("@EmployeeID", SqlDbType.Int) { Value = employeeId });

            // Execute and handle result
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new EmployeeCompany
                {
                    Id = reader.GetSafeInt32("Id"),
                    EmployeesAccountId = reader.GetSafeInt32("EmployeesAccountId"),
                    CompanyId = reader.GetSafeInt32("CompanyId"),
                    FullName = reader.GetSafeString("FullName"),
                    Alias = reader.GetSafeString("Alias"),
                    Prefix = reader.GetSafeString("Prefix"),
                    IsActive = reader.GetSafeBoolean("IsActive")
                });
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployeeCompany Exception.", ex);
            throw;
        }
        finally
        {
            if (connection.State == ConnectionState.Open)
                await connection.CloseAsync();
        }

        return result;
    }
}