using System.Net;
using System.Data;
using Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Shared.Helpers;
using Shared.Extensions;
using Company.Entities;
using Infrastructure.StoredProcedureMapperModule;

namespace Company.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly DatabaseConnection _dbConnection;
    private readonly StoredProcedureMapperModule _storedProcedureMapper;

    public CompanyRepository(DatabaseConnection dbConnection)
    {
        _dbConnection = dbConnection;
        _storedProcedureMapper = new StoredProcedureMapperModule();
    }

    public async Task<int> CreateCompanyAsync(string fullName, string address)
    {
        using var connection = _dbConnection.CreateConnection("Default");
        var companyId = 0;

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@FullName", fullName },
                { "@Addess", address }
            };

            var outputParameters = new Dictionary<string, object>();
            var success = await _storedProcedureMapper.ExecuteStoredProcedureAsync(connection, "Ins_Company_Create", parameters, outputParameters);

            if (success && outputParameters.ContainsKey("@CompanyId"))
            {
                companyId = Convert.ToInt32(outputParameters["@CompanyId"]);
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("CreateCompany Exception.", ex);
            throw;
        }

        return companyId;
    }

    public async Task<IEnumerable<EmployeeCompany>> GetEmployeeCompanyAsync(int employeeId)
    {
        using var connection = _dbConnection.CreateConnection("Default");
        var result = new List<EmployeeCompany>();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@EmployeeID", employeeId }
            };

            var dataTable = await _storedProcedureMapper.ExecuteStoredProcedureWithResultAsync(connection, "Ins_Company_GetEmployeeID", parameters);

            if (dataTable != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result.Add(new EmployeeCompany
                    {
                        Id = row.GetSafeInt32("Id"),
                        EmployeesAccountId = row.GetSafeInt32("EmployeesAccountId"),
                        CompanyId = row.GetSafeInt32("CompanyId"),
                        FullName = row.GetSafeString("FullName"),
                        Alias = row.GetSafeString("Alias"),
                        Prefix = row.GetSafeString("Prefix"),
                        IsActive = row.GetSafeBoolean("IsActive")
                    });
                }
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployeeCompany Exception.", ex);
            throw;
        }

        return result;
    }
}