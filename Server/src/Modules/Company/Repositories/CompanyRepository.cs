using System.Data;
using Infrastructure.DbContext;
using Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Shared.Helpers;
using Shared.Extensions;
using Company.Entities;

namespace Company.Repositories;

public class CompanyRepository : BaseRepository, ICompanyRepository
{
    public CompanyRepository(DatabaseConnection dbConnection) 
        : base(dbConnection, "TanCa")
    {
    }

    public async Task<int> CreateCompanyAsync(string fullName, string address)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@FullName", fullName },
                { "@Addess", address }
            };

            return await ExecuteStoredProcedureAsync("Ins_Company_Create", parameters);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error("CreateCompany Exception.", ex);
            return 0;
        }
    }

    public async Task<IEnumerable<EmployeeCompany>> GetEmployeeCompanyAsync(int employeeId)
    {
        var result = new List<EmployeeCompany>();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@EmployeeID", employeeId }
            };

            var dataTable = await ExecuteStoredProcedureWithResultAsync("Ins_Company_GetEmployeeID", parameters);

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
        }

        return result;
    }
}