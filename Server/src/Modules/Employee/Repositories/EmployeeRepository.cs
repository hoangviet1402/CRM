using EmployeeModule.DTOs;
using EmployeeModule.Entities;
using Infrastructure.DbContext;
using Infrastructure.StoredProcedureMapperModule;
using Microsoft.Data.SqlClient;
using System.Data;
using Shared.Result;
using Shared.Helpers;
using Shared.Enums;

namespace EmployeeModule.Repositories;

public class EmployeeRepository : StoredProcedureMapperModule, IEmployeeRepository
{
    public EmployeeRepository(DatabaseConnection dbConnection)
        : base(dbConnection, "TanCa")
    {
    }

    public async Task<EmployeeCreate_ResultEntities> EmployeeRegister(string fullname, string employeesCode, string phone, string email, string password, int companyId, int role)
    {
        var response = new EmployeeCreate_ResultEntities();

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@FullName", fullname },
                { "@EmployeesCode", employeesCode },
                { "@Phone", phone },
                { "@Email", email },
                { "@Password", string.IsNullOrEmpty(password) ? "" : AESHelper.HashPassword(password) },
                { "@CompanyId", companyId },
                { "@Role", role }
            };

            var outputParameters = new Dictionary<string, object>();
            var success = await ExecuteStoredProcedureAsync<bool>("Ins_Employee_Create", parameters, outputParameters);

            if (success)
            {
                response.EmployeeAccountId = outputParameters.GetSafeInt32("@EmployeeAccountId");
                response.IsNewUser = outputParameters.GetSafeInt32("@IsNewUser");
                response.NeedSetPassword = outputParameters.GetSafeInt32("@NeedSetPassword");
                response.NeedSetCompany = outputParameters.GetSafeInt32("@NeedSetCompany");
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"EmployeeRegister Exception.", ex);
        }

        return response;
    }

    public async Task<EmployeeEntity?> GetEmployeeById(int id, int companyId)
    {
        EmployeeEntity? result = null;

        try
        {
            var parameters = new Dictionary<string, object>
            {
                { "@EmployeeId", id },
                { "@CompanyId", companyId }
            };

            var dataTable = await ExecuteStoredProcedureAsync<DataTable>("Ins_Employee_GetById", parameters);

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                var row = dataTable.Rows[0];
                result = new EmployeeEntity
                {
                    Id = row.GetSafeInt32("Id"),
                    FullName = row.GetSafeString("FullName"),
                    EmployeeCode = row.GetSafeString("EmployeeCode"),
                    Email = row.GetSafeString("Email"),
                    Phone = row.GetSafeString("Phone")
                };
            }
        }
        catch (Exception ex)
        {
            LoggerHelper.Error($"GetEmployeeById Exception.", ex);
        }

        return result;
    }
} 